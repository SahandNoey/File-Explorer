using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FileExplorer
{
    public enum Options
    {
        SearchForFiles = 1,
        ManageExtensions,
        ViewSearchHistory,
        Exit
    }

    public enum SearchType
    {
        ByName = 1,
        ByContent = 2
    }
    public class InputHandler
    {
        private string UserInput { get; set; }
        public List<string> InputTypes { get; set; }
        public string InputPath { get; set; }
        public string InputPathExtension { get; set; }
        public string InputQuery { get; set; }
        public SearchType SearchType { get; set; }
        
        public bool TxtExtensionLoaded { get; set; }
        public bool JsonExtensionLoaded { get; set; }
        public bool ViewSearchHistory { get; set; }

        private Options ChosenOption { get; set; }
        public InputHandler()
        {
            Console.WriteLine("=== BOOTCAMP SEARCH :: An extendible command-line search tool ===\n");
        }
        
        // Print program options
        public void WelcomePrinter()
        {
            while (true)
            {
                Console.WriteLine("1. Search for files\n2. Manage Extensions\n3. View search history\n\n4. Exit\n");
                Console.Write("> Please input an option: ");
                this.UserInput = Console.ReadLine();
                if (int.TryParse(this.UserInput, out int chosenOption))
                {
                    if (!Enum.IsDefined(typeof(Options), chosenOption))
                    {
                        Console.Write("Incorrect input!\n");
                    }
                    else
                    {
                        PrintHandler();
                        break;
                        
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input!\n");
                }
            }
        }
        
        // Handle user option
        private void PrintHandler()
        {
            var optionInt = int.Parse(this.UserInput);
            var optionEnum = (Options)optionInt;
            switch (optionEnum)
            {
                case Options.SearchForFiles:
                    this.ChosenOption = Options.SearchForFiles;
                    SearchPrinter();
                    break;
                case Options.ManageExtensions:
                    this.ChosenOption = Options.ManageExtensions;
                    ManagePrinter();
                    break;
                case Options.ViewSearchHistory:
                    this.ChosenOption = Options.ViewSearchHistory;
                    ViewPrinter();
                    break;
                case Options.Exit:
                    this.ChosenOption = Options.Exit;
                    ExitPrinter();
                    break;
            }
        }

        
        private void SearchPrinter()
        {
            // Search type selection
            var supportedTypesByName = new SearchForFiles().SupportedTypesByNames;
            var supportedTypesByContent = new SearchForFiles().SupportedTypesByContent;
               
            Console.WriteLine("> Select search type:");
            Console.WriteLine("1.Search by file name\n2.Advanced search");
            var searchTypeInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(searchTypeInput))
            {
                this.SearchType = (SearchType)int.Parse(searchTypeInput);
            }

            
            List<string> fileTypes = new List<string>();
            bool incorrectInput = true;
            while (incorrectInput)
            {
                incorrectInput = false;
                if (this.SearchType == SearchType.ByName)
                {
                    Console.Write("> Input file types you want to search seperated by comma(,)(Most common types are supported)");
                    Console.WriteLine(" (press enter if you want search within any types)");
                    Console.WriteLine("Example1 --> TXT, JSON, XML");
                    Console.WriteLine("Example2 --> TXT\n");
                } else if (this.SearchType == SearchType.ByContent)
                {
                    Console.WriteLine("> Input file type you want to search(Supported types: TXT):");
                    foreach (var type in supportedTypesByContent)
                    {
                        Console.Write($"{type}, ");
                    }
                    Console.WriteLine();
                }
                Console.Write("> Your Input: ");
                this.UserInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(this.UserInput))
                {
                    this.UserInput = this.UserInput.Replace(" ", "")
                        .Replace("\t", "")
                        .Replace("\n", "");
                    this.UserInput = this.UserInput.ToLower();
                    fileTypes = new List<string>(this.UserInput.Split(','));

                    if (this.SearchType == SearchType.ByName)
                    {
                        foreach (var type in fileTypes)
                        {
                            if (!supportedTypesByName.Contains(type))
                            {
                                Console.WriteLine("Incorrect input!");
                                incorrectInput = true;
                                break;
                            }
                        }
                        this.InputTypes = fileTypes;
                    } else if (this.SearchType == SearchType.ByContent)
                    {
                        this.InputTypes = new List<string> { fileTypes[0] };
                        foreach (var type in fileTypes)
                        {
                            if (!supportedTypesByContent.Contains(type))
                            {
                                Console.WriteLine("Incorrect input!");
                                incorrectInput = true;
                                break;
                            }
                        }

                        if (fileTypes[0] == "txt" && this.TxtExtensionLoaded == false)
                        {
                            Console.WriteLine("First load txt extension please by choosing manage extension");
                            Thread.Sleep(2000);
                            WelcomePrinter();
                        } else if (fileTypes[0] == "json" && this.JsonExtensionLoaded == false)
                        {
                            Console.WriteLine("First load json extension please by choosing manage extension");
                            Thread.Sleep(2000);
                            WelcomePrinter();
                        }
                    }
                }
            }
            
            while (true)
            {
                Console.Write("> Pick the root path: ");
                this.InputPath = Console.ReadLine();
                if (!Directory.Exists(this.InputPath))
                {
                    Console.WriteLine("System.IO.DirectoryNotFoundException: Directory does not exist!");
                    continue;
                }
                break;
            }



            if (this.SearchType == SearchType.ByName)
            {
                Console.Write("> Query(press enter for searching every file names with your selected format(s)): ");

            }
            else
            {
                Console.Write("> Enter a word existing in your specific text file: ");
            }
            this.InputQuery = Console.ReadLine();
        }
        
        private void ManagePrinter()
        {
            while (true)
            {
                Console.Write("> Input your plugin folder path: ");
                this.InputPath = Console.ReadLine();
                if (!Directory.Exists(this.InputPath))
                {
                    Console.WriteLine("Incorrect path!");
                    continue;
                }
                break;
            }

            while (true)
            {
                Console.WriteLine("> Select which extension you want to load");
                var extensionFiles = Directory.GetFiles(this.InputPath, "*.dll");
                for (int i = 0; i < extensionFiles.Length; i++)
                {
                    Console.WriteLine($"{i+1}.{extensionFiles[i]}");
                }
                
                var extensionNumberStr = Console.ReadLine();
                if (string.IsNullOrEmpty(extensionNumberStr))
                {
                    continue;
                }
                var extensionNumber = int.Parse(extensionNumberStr);
                if (extensionNumber > extensionFiles.Length)
                {
                    Console.WriteLine("Wrong Input!");
                    continue;
                }
                this.InputPathExtension = extensionFiles[extensionNumber - 1];
                if (extensionNumber == 1)
                {
                    this.TxtExtensionLoaded = true;
                }
                else if(extensionNumber == 2)
                {
                    this.JsonExtensionLoaded = true;
                }
                break;
            }
            Console.WriteLine("Extension loaded successfully...");
            Thread.Sleep(1000);
            WelcomePrinter();
        }

        private void ViewPrinter()
        {
            this.ViewSearchHistory = true;
            Console.WriteLine("> Here is your search history:");
        }

        private void ExitPrinter()
        {
            Console.WriteLine("Thanks for using this program :)");
            Environment.Exit(0);
        }
    }
}