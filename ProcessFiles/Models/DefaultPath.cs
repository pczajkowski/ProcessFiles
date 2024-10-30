using System.IO;
using FileOperations.Interfaces;

namespace FileOperations.Models
{
    public class DefaultPath : IPath
    {
        public string? GetExtension(string? path) => Path.GetExtension(path);
    }
}
