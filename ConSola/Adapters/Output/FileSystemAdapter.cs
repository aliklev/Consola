using ConSola.Core.Domain;
using ConSola.Core.Ports.Output;

namespace ConSola.Adapters.Output;

public class FileSystemAdapter : IFileSystemPort
{
    public async Task<List<FileItem>> GetDirectoryContentsAsync(string path)
    {
        var items = new List<FileItem>();
        
        if (Directory.GetParent(path) is not null)
            items.Add(new FileItem { Name = "..", Path = Directory.GetParent(path)!.FullName, IsDirectory = true });

        await Task.Run(() =>
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                var dirInfo = new DirectoryInfo(dir);
                items.Add(new FileItem 
                { 
                    Name = dirInfo.Name, 
                    Path = dir, 
                    IsDirectory = true,
                    Modified = dirInfo.LastWriteTime
                });
            }

            foreach (var file in Directory.GetFiles(path))
            {
                var fileInfo = new FileInfo(file);
                items.Add(new FileItem 
                { 
                    Name = fileInfo.Name, 
                    Path = file, 
                    IsDirectory = false,
                    Size = fileInfo.Length,
                    Modified = fileInfo.LastWriteTime
                });
            }
        });

        return items.OrderBy(x => !x.IsDirectory).ThenBy(x => x.Name).ToList();
    }

    public async Task CopyItemAsync(string source, string destination)
    {
        await Task.Run(() =>
        {
            if (Directory.Exists(source))
                CopyDirectory(source, destination);
            else
                File.Copy(source, destination, true);
        });
    }

    public async Task MoveItemAsync(string source, string destination)
    {
        await Task.Run(() =>
        {
            if (Directory.Exists(source))
                Directory.Move(source, destination);
            else
                File.Move(source, destination, true);
        });
    }

    public async Task DeleteItemAsync(string path)
    {
        await Task.Run(() =>
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            else
                File.Delete(path);
        });
    }

    public bool DirectoryExists(string path) => Directory.Exists(path);
    public bool FileExists(string path) => File.Exists(path);

    public string[] GetAvailableDrives()
        => DriveInfo.GetDrives().Where(d => d.IsReady).Select(d => d.Name).ToArray();

    private void CopyDirectory(string src, string dest)
    {
        Directory.CreateDirectory(dest);
        foreach (var file in Directory.GetFiles(src))
            File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
        foreach (var dir in Directory.GetDirectories(src))
            CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
    }
}