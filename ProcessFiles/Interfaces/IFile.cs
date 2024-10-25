using System.IO;

namespace ProcessFiles.Interfaces
{
    public interface IFile
    {
        public FileAttributes GetAttributes(string path);
    }
}
