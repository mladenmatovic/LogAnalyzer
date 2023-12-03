using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer
{
    public class IpAddressHelper
    {
        public static IPAddress ExtractIPAddressFromLineString(string s, int clientIpColumnIndex)
        {
            return Parse(s.Split(' ')[clientIpColumnIndex]);
        }

        // https://learn.microsoft.com/en-us/dotnet/api/system.net.ipaddress.parse?view=net-8.0&redirectedfrom=MSDN#System_Net_IPAddress_Parse_System_String_
        private static IPAddress Parse(string ipAddress)
        {
            try
            {
                IPAddress address = IPAddress.Parse(ipAddress);

                return address;
            }

            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }

            catch (FormatException e)
            {
                Console.WriteLine("FormatException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }

            return IPAddress.Any;
        }
    }
}
