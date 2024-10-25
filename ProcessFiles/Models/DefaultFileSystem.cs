using ProcessFiles.Interfaces;

namespace ProcessFiles.Models
{
    public class DefaultFileSystem : IFileSystem
    {
        private readonly IFile file = new DefaultFile();
        public IFile File => file;

        private readonly IDirectory directory = new DefaultDirectory();
        public IDirectory Directory => directory;

        private readonly IPath path = new DefaultPath();
        public IPath Path => path;
    }
}
