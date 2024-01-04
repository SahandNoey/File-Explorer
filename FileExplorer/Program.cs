using System;
using System.Collections.Generic;
using System.Threading;

namespace FileExplorer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            InputHandler inputHandler = new InputHandler();
            var searchHistory = new HashSet<string>();
            var count = 1;
            while (true)
            {
                inputHandler.WelcomePrinter();
                SearchForFiles search = new SearchForFiles();
                var result = new List<string>();
                if (inputHandler.SearchType == SearchType.ByName)
                {
                    result = search.SearchHandle(inputHandler.InputTypes,
                        inputHandler.InputPath, inputHandler.InputQuery);
                }
                else if(inputHandler.SearchType == SearchType.ByContent)
                {
                    ExtensionHandler extensionHandler = new ExtensionHandler();
                    var loadedExtension = extensionHandler.ManageExtension(inputHandler.InputPathExtension);
                    result = loadedExtension.ContentSearch(inputHandler.InputTypes[0], inputHandler.InputPath,
                        inputHandler.InputQuery);
                }
                if (inputHandler.ViewSearchHistory == true)
                {
                    if (searchHistory.Count == 0)
                    {
                        Console.WriteLine("You didn't search anything until now!");
                        continue;
                    }
                    inputHandler.ViewSearchHistory = false;
                    Console.WriteLine(string.Join("\n", searchHistory));
                    continue;
                }
                var printResult = string.Join("\n", result);
                searchHistory.Add($"{count++}.File Type(s):{string.Join(",", inputHandler.InputTypes)}\tQuery:{inputHandler.InputQuery}\n" +
                                  $"\tResult(s):{{{string.Join("\n\t\t", result)}}}\n");
                Console.WriteLine(printResult);
                
                Thread.Sleep(2000);
                Console.WriteLine();
            }
            
        }
    }
}