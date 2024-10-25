using ProcessFiles.Interfaces;
using System.IO;

namespace ProcessFiles.Models
{
    public class DefaultPath : IPath
    {
        public string? GetExtension(string? path) => Path.GetExtension(path);
    }
}
