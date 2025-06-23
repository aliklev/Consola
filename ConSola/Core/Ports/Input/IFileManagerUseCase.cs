using ConSola.Core.Domain;

namespace ConSola.Core.Ports.Input;

public interface IFileManagerUseCase
{
    Task<List<FileItem>> GetDirectoryContentsAsync(string path);
    Task CopyFileAsync(string source, string destination);
    Task MoveFileAsync(string source, string destination);
    Task DeleteFileAsync(string path);
    string[] GetAvailableDrives();
}