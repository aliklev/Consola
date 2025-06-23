namespace ConSola.Core.Ports.Input;

public interface INavigationUseCase
{
    string GetCurrentPath();
    string GetSelectedItemPath();
    string GetInactivePanelPath();
    void NavigateToPath(string path);
    void NavigateInto();
    void NavigateBack();
    void SwitchPanel();
    void RefreshCurrentPanel();
}