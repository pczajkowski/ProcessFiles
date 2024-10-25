using ProcessFiles.Interfaces;
using System.IO;

namespace ProcessFiles.Models
{
    public class DefaultDirectory : IDirectory
    {
        public bool Exists(string? path) => Directory.Exists(path);
    }
}
