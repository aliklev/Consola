using ConSola.Core.Domain;
using ConSola.Core.Ports.Input;
using ConSola.Core.Ports.Output;

namespace ConSola.Core.UseCases;

public class FileManagerService : IFileManagerUseCase
{
    private readonly IFileSystemPort _fileSystemPort;
    private readonly IUserInterfacePort _uiPort;

    public FileManagerService(IFileSystemPort fileSystemPort, IUserInterfacePort uiPort)
    {
        _fileSystemPort = fileSystemPort;
        _uiPort = uiPort;
    }

    public async Task<List<FileItem>> GetDirectoryContentsAsync(string path)
        => await _fileSystemPort.GetDirectoryContentsAsync(path);

    public async Task CopyFileAsync(string source, string destination)
    {
        await _fileSystemPort.CopyItemAsync(source, destination);
        _uiPort.ShowMessage("Success", "Copy completed");
    }

    public async Task MoveFileAsync(string source, string destination)
    {
        await _fileSystemPort.MoveItemAsync(source, destination);
        _uiPort.ShowMessage("Success", "Move completed");
    }

    public async Task DeleteFileAsync(string path)
    {
        await _fileSystemPort.DeleteItemAsync(path);
        _uiPort.ShowMessage("Success", "Delete completed");
    }

    public string[] GetAvailableDrives() => _fileSystemPort.GetAvailableDrives();
}