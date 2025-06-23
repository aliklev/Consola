namespace ConSola.Core.Ports.Output;

public interface IUserInterfacePort
{
    void Render();
    void ShowMessage(string title, string message);
    bool ShowConfirmation(string title, string message);
    string? ShowDriveSelector(string[] drives);
    void ShowHelp();
    void Shutdown();
}