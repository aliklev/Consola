namespace ConSola.Core.Domain;

public class FileItem
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDirectory { get; set; }
    public string Attributes { get; set; } = string.Empty;
}