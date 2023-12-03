using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer
{
    public class LogProcessor
    {
        public static async Task ProcessLogFile(string path)
        {
            int clientIpColumnIndex = -1;
            Dictionary<IPAddress, int> clientLog = new();
            // using concurrent dictionary as DNS retrieval is done in async way 
            ConcurrentDictionary<IPAddress, IPHostEntry> clientAddress = new();
            List<Task> tasks = new();

            // https://stackoverflow.com/questions/2161895/reading-large-text-files-with-streams-in-c-sharp
            using (StreamReader sr = File.OpenText(path))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.StartsWith("#"))
                    {
                        if (s.StartsWith("#Fields:"))
                        {
                            clientIpColumnIndex = FindClientIpColumnIndex(s, "c-ip");
                        }

                        continue;
                    }

                    IPAddress ip = IpAddressHelper.ExtractIPAddressFromLineString(s, clientIpColumnIndex);
                    // https://www.codeproject.com/Questions/716560/How-Do-I-Read-And-Process-Big-Log-File-In-Csharp
                    if (!clientLog.ContainsKey(ip))
                    {
                        clientLog.Add(ip, 0);
                        //Console.WriteLine($"Adding the new address {ip}");

                        // start getting DNS lookups, but all will be awaited later
                        tasks.Add(AddHostToAddressDictionaryAsync(ip, clientAddress));
                    }
                    clientLog[ip]++;
                }

                var sortedLogDict = from entry in clientLog orderby entry.Value descending select entry;

                // wait for all DNS lookup to finish
                await Task.WhenAll(tasks);

                PrintLogToConsole(sortedLogDict, clientAddress);                
            }
        }

        private static void PrintLogToConsole(IOrderedEnumerable<KeyValuePair<IPAddress, int>> logDictionary, ConcurrentDictionary<IPAddress, IPHostEntry> clientAddress)
        {
            foreach (KeyValuePair<IPAddress, int> kvp in logDictionary)
            {
                Console.WriteLine($"{clientAddress[kvp.Key].HostName} ({kvp.Key}) - {kvp.Value}");
            }
        }

        private static async Task AddHostToAddressDictionaryAsync(IPAddress ip, ConcurrentDictionary<IPAddress, IPHostEntry> clientAddress)
        {
            var host = await GetHostEntryAsync(ip);
            //Console.WriteLine($"Found host entry {host.HostName} for IP {ip}");
            clientAddress.TryAdd(ip, host);
        }

        private static async Task<IPHostEntry> GetHostEntryAsync(IPAddress ip)
        {
            IPHostEntry host;
            try
            {
                host = await Dns.GetHostEntryAsync(ip);
            }
            catch (SocketException)
            {
                //Console.WriteLine($"No host address found");
                host = new IPHostEntry();
                host.HostName = "Not available";
            }

            return host;
        }

        public static int FindClientIpColumnIndex(string s, string columnName)
        {
            return Array.FindIndex(s.Split(' '), s => s == columnName) - 1;
        }
    }
}
