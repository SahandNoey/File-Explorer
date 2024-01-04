using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SearchInterface;

namespace TextContentSearch
{
    public class TextSearch: ISearch
    {
        public List<string> ContentSearch(string inputType, string inputPath, string inputQuery)
        {
            inputQuery = inputQuery.ToLower();
            var txtFilesStrArr = Directory.GetFiles(inputPath, "*.txt", SearchOption.AllDirectories);
            var txtFiles = txtFilesStrArr.ToList();
            var result = new List<string>();
            var tasks = new List<Task>();
            for (int i = 0; i < Math.Ceiling(txtFiles.Count / 3.0); i++)
            {
                var tmp = i;
                tasks.Add(Task.Run(() =>
                {
                    var taskFilesQouta = txtFiles.GetRange(3 * tmp, Math.Min(3, txtFiles.Count - 3 * tmp));
                    foreach (var txtFile in taskFilesQouta)
                    {
                        var fileContent = File.ReadAllText(txtFile);
                        fileContent = fileContent.ToLower();
                        if (fileContent.Contains(inputQuery))
                        {
                            result.Add(txtFile);
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            if (result.Count == 0)
            {
                Console.WriteLine("Doesn't exist");
            }
            return result;
        }
    }
}