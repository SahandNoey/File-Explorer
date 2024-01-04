using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer
{
    public class SearchForFiles
    {
        public List<string> SupportedTypesByNames { get; set; }
        public List<string> SupportedTypesByContent { get; set; }
        
        private string InputPath { get; set; }
        public SearchForFiles()
        {
            this.SupportedTypesByNames = new List<string>()
            {
                "xml", "txt", "pdf", "json", "bin", "exe", "bin", "avi", "bmp", "csv", "dll", "doc", "docx", "exe",
                "html", "jpg", "jpeg", "mp3", "mp4", "mpeg", "mpg", "png", "ppt", "pptx", "rar", "zip", "wav", "wmv",
                "xls", "xlsx", "accdb"
            };
            this.SupportedTypesByContent = new List<string>() {"txt"};
        }

        public List<string> SearchHandle(List<string> inputTypes, string inputPath, string inputQuery)
        {
            this.InputPath = inputPath;

            // Multithread searching
            var dirs = Directory.GetDirectories(inputPath);
            var dirsList = dirs.ToList();

            var searchTasks = new Task<List<string>>[(int)Math.Ceiling(dirs.Length / 3.0 + (1 / 3.0))];

            var firstThreadDirs = dirsList.GetRange(0, Math.Min(3, dirs.Length));
            firstThreadDirs.Add(inputPath);
            searchTasks[0] = Task.Run(() => SearchByName(inputTypes,
                firstThreadDirs, inputQuery));

            for (int i = 1; i < (int)Math.Ceiling(dirs.Length / 3.0); i++)
            {
                if (dirsList.Count > 3)
                {
                    int tmp = i;
                    List<string> ip = dirsList.GetRange(3 * tmp, Math.Min(3, dirs.Length - 3 * tmp));
                    // Console.WriteLine(ip.Count);
                    searchTasks[tmp] = Task.Run(() => SearchByName(inputTypes, ip, inputQuery));
                }
            }

            Task.WhenAll(searchTasks).Wait();

            List<string> results = new List<string>();
            for (int i = 0; i < (int)Math.Ceiling(dirs.Length / 3.0); i++)
            {
                results.AddRange(searchTasks[i].Result);
            }

            Console.WriteLine();
            if (results.Count == 0)
            {
                results.Add("File doesn't exist");
            }

            return results;
        }

        private List<string> SearchByName(List<string> inputTypes, List<string> inputPaths, string inputQuery)
        {
            // Console.WriteLine($"length of input paths = {inputPaths.Count} from thread {Thread.CurrentThread.ManagedThreadId}");
            List<string> result = new List<string>();
            foreach (var inputPath in inputPaths)
            {
                if (string.IsNullOrEmpty(inputQuery) && inputTypes.Count == 0)
                {
                    try
                    {
                        if (inputPath == this.InputPath)
                        {
                            result.AddRange(Directory.GetFiles(inputPath, "*.*", SearchOption.TopDirectoryOnly));
                            continue;
                        }
                        result.AddRange(Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: Access Denied!");
                        Console.WriteLine(
                            "If you want to search throw these directories you must have administrator permission");
                    }
                } else if (!string.IsNullOrEmpty(inputQuery) && inputTypes.Count == 0)
                {
                    foreach (var supportedTypesByName in this.SupportedTypesByNames)
                    {
                        try
                        {
                            if (inputPath == this.InputPath)
                            {
                                result.AddRange(Directory.GetFiles(inputPath, $"*{inputQuery}*.{supportedTypesByName}", SearchOption.TopDirectoryOnly));
                                continue;
                            }
                            result.AddRange(Directory.GetFiles(inputPath,
                                $"*{inputQuery}*.{supportedTypesByName}", SearchOption.AllDirectories));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: Access Denied!");
                            Console.WriteLine("If you want to search throw these directories you must have administrator permission");
                        }
                        
                    }
                } else if (string.IsNullOrEmpty(inputQuery) && inputTypes.Count != 0)
                {
                    foreach (var inputType in inputTypes)
                    {
                        try
                        {
                            if (inputPath == this.InputPath)
                            {
                                result.AddRange(Directory.GetFiles(inputPath, $"*.{inputType}", SearchOption.TopDirectoryOnly));
                                continue;
                            }
                            result.AddRange(Directory.GetFiles(inputPath,
                                $"*.{inputType}", SearchOption.AllDirectories));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: Access Denied!");
                            Console.WriteLine("If you want to search throw these directories you must have administrator permission");
                        }
                    }
                } else if (!string.IsNullOrEmpty(inputQuery) && inputTypes.Count != 0)
                {
                    foreach (var inputType in inputTypes)
                    {
                        try
                        {
                            if (inputPath == this.InputPath)
                            {
                                result.AddRange(Directory.GetFiles(inputPath, $"*{inputQuery}*.{inputType}", SearchOption.TopDirectoryOnly));
                                continue;
                            }
                            result.AddRange(Directory.GetFiles(inputPath,
                                $"*{inputQuery}*.{inputType}", SearchOption.AllDirectories));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: Access Denied!");
                            Console.WriteLine("If you want to search throw these directories you must have administrator permission");
                        }
                    }
                }
            }
            // Console.WriteLine($"{result.ToString()} from thread {Thread.CurrentThread.ManagedThreadId}");
            return result;
        }
        
        
    }
}