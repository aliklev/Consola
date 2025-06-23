using ConSola.Core.Domain;

namespace ConSola.Core.Ports.Output;

public interface IFileSystemPort
{
    Task<List<FileItem>> GetDirectoryContentsAsync(string path);
    Task CopyItemAsync(string source, string destination);
    Task MoveItemAsync(string source, string destination);
    Task DeleteItemAsync(string path);
    bool DirectoryExists(string path);
    bool FileExists(string path);
    string[] GetAvailableDrives();
}