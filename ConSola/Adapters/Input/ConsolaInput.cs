using ConSola.Adapters.Output;
using Terminal.Gui;

namespace ConSola.Adapters.Input;

public class ConsolaInput
{
    private readonly ConsolaUI _ui;

    public ConsolaInput(ConsolaUI ui)
    {
        _ui = ui;
        SetupKeyBindings();
    }

    public void SetupKeyBindings()
    {
        Application.RootKeyEvent = (KeyEvent keyEvent) =>
        {
            return keyEvent.Key switch
            {
                // Navigation
                Key.Enter => HandleEnter(),
                Key.Backspace => HandleBackspace(),
                Key.Tab => HandleTab(),
                
                // File Operations
                Key.F3 => HandleF3(),
                Key.F5 => HandleF5(),
                Key.F6 => HandleF6(),
                Key.F8 or Key.DeleteChar => HandleF8(),
                
                // System
                Key.F1 => HandleF1(),
                Key.F10 or Key.Esc => HandleF10(),
                Key.CtrlMask | Key.D => HandleCtrlD(),
                
                _ => false
            };
        };
    }

    private bool HandleEnter()
    {
        _ui.NavigateInto();
        return true;
    }

    private bool HandleBackspace()
    {
        _ui.NavigateUp();
        return true;
    }

    private bool HandleTab()
    {
        _ui.SwitchPanel();
        return true;
    }

    private bool HandleF1()
    {
        _ui.ShowHelp();
        return true;
    }

    private bool HandleF3()
    {
        _ui.ShowSearch();
        return true;
    }

    private bool HandleF5()
    {
        var source = _ui.GetSelectedItem();
        var dest = _ui.GetInactivePanelPath();
        
        if (!string.IsNullOrEmpty(source))
        {
            var fileName = Path.GetFileName(source);
            var isDirectory = Directory.Exists(source);
            var itemType = isDirectory ? "folder" : "file";
            
            if (_ui.ShowConfirmation($"Copy {itemType}", $"Are you sure you want to copy this {itemType}?\n\nFrom: {source}\nTo: {Path.Combine(dest, fileName)}\n\nThis will copy the {itemType} to the other panel."))
            {
                try
                {
                    var destPath = Path.Combine(dest, fileName);
                    if (isDirectory)
                        CopyDirectory(source, destPath);
                    else
                        File.Copy(source, destPath, true);
                    
                    _ui.ShowMessage("Copy Successful", $"'{fileName}' copied successfully!");
                    _ui.NavigateToPath(_ui.GetCurrentPath());
                }
                catch (Exception ex)
                {
                    _ui.ShowMessage("Copy Error", $"Failed to copy '{fileName}':\n{ex.Message}");
                }
            }
        }
        else
        {
            _ui.ShowMessage("No Selection", "Please select a file or folder to copy.");
        }
        return true;
    }

    private bool HandleF6()
    {
        var source = _ui.GetSelectedItem();
        var dest = _ui.GetInactivePanelPath();
        
        if (!string.IsNullOrEmpty(source))
        {
            var fileName = Path.GetFileName(source);
            var isDirectory = Directory.Exists(source);
            var itemType = isDirectory ? "folder" : "file";
            
            if (_ui.ShowConfirmation($"Move {itemType}", $"Are you sure you want to move this {itemType}?\n\nFrom: {source}\nTo: {Path.Combine(dest, fileName)}\n\nThis will move the {itemType} to the other panel."))
            {
                try
                {
                    var destPath = Path.Combine(dest, fileName);
                    if (isDirectory)
                        Directory.Move(source, destPath);
                    else
                        File.Move(source, destPath, true);
                    
                    _ui.ShowMessage("Move Successful", $"'{fileName}' moved successfully!");
                    _ui.NavigateToPath(_ui.GetCurrentPath());
                }
                catch (Exception ex)
                {
                    _ui.ShowMessage("Move Error", $"Failed to move '{fileName}':\n{ex.Message}");
                }
            }
        }
        else
        {
            _ui.ShowMessage("No Selection", "Please select a file or folder to move.");
        }
        return true;
    }

    private bool HandleF8()
    {
        var source = _ui.GetSelectedItem();
        
        if (!string.IsNullOrEmpty(source))
        {
            var fileName = Path.GetFileName(source);
            var isDirectory = Directory.Exists(source);
            var itemType = isDirectory ? "folder" : "file";
            
            if (_ui.ShowConfirmation($"Delete {itemType}", $"Are you sure you want to delete this {itemType}?\n\n'{fileName}'\n\nThis action cannot be undone!"))
            {
                try
                {
                    if (isDirectory)
                        Directory.Delete(source, true);
                    else
                        File.Delete(source);
                    
                    _ui.ShowMessage("Delete Successful", $"'{fileName}' deleted successfully!");
                    // Refresh current panel
                    _ui.NavigateToPath(_ui.GetCurrentPath());
                }
                catch (Exception ex)
                {
                    _ui.ShowMessage("Delete Error", $"Failed to delete '{fileName}':\n{ex.Message}");
                }
            }
        }
        else
        {
            _ui.ShowMessage("No Selection", "Please select a file or folder to delete.");
        }
        return true;
    }

    private bool HandleF10()
    {
        if (_ui.ShowConfirmation("Exit ConSola", "Are you sure you want to exit ConSola File Manager?"))
        {
            _ui.Shutdown();
        }
        return true;
    }

    private bool HandleCtrlD()
    {
        var drives = DriveInfo.GetDrives()
            .Where(d => d.IsReady)
            .Select(d => $"{d.Name} - {d.DriveType} ({GetDriveSize(d)})")
            .ToArray();
            
        var selected = _ui.ShowDriveSelector(drives);
        if (selected != null)
        {
            var driveLetter = selected.Split(' ')[0];
            _ui.NavigateToPath(driveLetter);
        }
        return true;
    }

    private string GetDriveSize(DriveInfo drive)
    {
        try
        {
            var totalGB = drive.TotalSize / (1024 * 1024 * 1024);
            var freeGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024);
            return $"{freeGB}GB free of {totalGB}GB";
        }
        catch
        {
            return "Size unknown";
        }
    }

    private void CopyDirectory(string src, string dest)
    {
        Directory.CreateDirectory(dest);
        foreach (var file in Directory.GetFiles(src))
            File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
        foreach (var dir in Directory.GetDirectories(src))
            CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
    }
}