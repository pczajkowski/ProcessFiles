using ProcessFiles.Interfaces;

namespace ProcessFiles.Models
{
    public class DefaultFileSystem : IFileSystem
    {
        private readonly IFile file = new DefaultFile();
        public IFile File => file;
    }
}
