using System.IO;

namespace FileOperations.Interfaces
{
    public interface IFile
    {
        public FileAttributes GetAttributes(string path);
        public bool Exists(string? path);
    }
}
