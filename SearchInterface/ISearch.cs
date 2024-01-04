using System.Collections.Generic;

namespace SearchInterface
{
    public interface ISearch
    {
        List<string> ContentSearch(string inputType, string inputPath, string inputQuery);
    }
}