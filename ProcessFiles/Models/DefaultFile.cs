using System.IO;
using ProcessFiles.Interfaces;

namespace ProcessFiles.Models
{
    public class DefaultFile : IFile
    {
        public FileAttributes GetAttributes(string path) => File.GetAttributes(path);
        public bool Exists(string? path) => File.Exists(path);
    }
}
