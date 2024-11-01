using System.IO;

namespace ProcessFiles.Interfaces
{
    public interface IDirectory
    {
        public bool Exists(string? path);
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
    }
}
