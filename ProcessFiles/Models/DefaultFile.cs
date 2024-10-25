using ProcessFiles.Interfaces;
using System.IO;

namespace ProcessFiles.Models
{
    public class DefaultFile : IFile
    {
        public FileAttributes GetAttributes(string path) => File.GetAttributes(path);
        public bool Exists(string? path) => File.Exists(path);
    }
}
