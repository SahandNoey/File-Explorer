using System.Reflection;
using SearchInterface;
using System;
using System.Linq;

namespace FileExplorer
{
    public class ExtensionHandler
    {
        public ISearch ManageExtension(string targetExtensionPath)
        {
            // Console.WriteLine(targetExtensionPath);
            Assembly externalAssembly = Assembly.LoadFrom(targetExtensionPath);
            var searchType = externalAssembly.GetTypes().Where(t => typeof(ISearch).IsAssignableFrom(t)).ToList();
            if (searchType.Count == 0)
            {
                Console.WriteLine("Extension is not valid!");
            }
            // Console.WriteLine($"Here: {searchType}");
            return (ISearch)Activator.CreateInstance(searchType[0]);
        }
    }
}