namespace ProcessFiles.Interfaces;

public interface IFileSystem
{
    public IFile File { get; }
    public IDirectory Directory { get; }
    public IPath Path { get; }
}