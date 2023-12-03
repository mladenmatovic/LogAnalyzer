using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer
{
    public class FileHelper
    {    
        public static List<string> GetLogFiles(string path)
        {
            List<string> logFiles = new();

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                Console.WriteLine("File/Folder path doesn't exist: " + path);
                return logFiles;
            }

            FileAttributes attr = File.GetAttributes(path);            

            if (attr.HasFlag(FileAttributes.Directory))
            {
                logFiles.AddRange(GetAllLogFiles(path));
            }
            else
            {
                Console.WriteLine("trying log file: " + path);
                logFiles.Add(path);
            }

            return logFiles;
        }

        //https://stackoverflow.com/questions/1395205/better-way-to-check-if-a-path-is-a-file-or-a-directory
        private static IReadOnlyList<string> GetAllLogFiles(string path)
        {
            List<string> logFiles = new();
            Console.WriteLine($"trying log path: {path}");
            if (Directory.Exists(path))
            {
                logFiles = Directory.GetFiles(path, "ex*.log").ToList();
                if (logFiles.Count == 0)
                {
                    Console.WriteLine($"No log files found at {path}");
                }                
            }
            else
            {
                Console.WriteLine($"path not found: {path}");
            }
            return logFiles.AsReadOnly();
        }        
    }
}
