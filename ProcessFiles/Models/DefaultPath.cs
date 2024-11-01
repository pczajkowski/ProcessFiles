using System.IO;
using ProcessFiles.Interfaces;

namespace ProcessFiles.Models
{
    public class DefaultPath : IPath
    {
        public string? GetExtension(string? path) => Path.GetExtension(path);
    }
}
