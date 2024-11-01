using System.IO;
using ProcessFiles.Interfaces;

namespace ProcessFiles.Models
{
    public class DefaultDirectory : IDirectory
    {
        public bool Exists(string? path) => Directory.Exists(path);
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
            => Directory.GetFiles(path, searchPattern, searchOption);
    }
}
