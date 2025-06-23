using ConSola.Core.Ports.Output;
using ConSola.Core.Domain;
using Microsoft.Extensions.Options;
using Terminal.Gui;

namespace ConSola.Adapters.Output;

public class ConsolaUI : IUserInterfacePort
{
    private ListView _leftList = null!;
    private ListView _rightList = null!;
    private FrameView _leftPanel = null!;
    private FrameView _rightPanel = null!;
    private Label _helpBar = null!;
    private Label _timeBar = null!;
    private string _leftPath = Directory.GetCurrentDirectory();
    private string _rightPath = Directory.GetCurrentDirectory();
    private int _activePanel = 0;
    private readonly UISettings _uiSettings;
    private readonly ApplicationSettings _appSettings;

    public ConsolaUI(IOptions<UISettings> uiSettings, IOptions<ApplicationSettings> appSettings)
    {
        _uiSettings = uiSettings.Value;
        _appSettings = appSettings.Value;
    }

    public void Render()
    {
        var window = new Window(_appSettings.Title)
        {
            X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill()
        };

        _leftPanel = new FrameView($"Left: {_leftPath}")
        {
            X = 0, Y = 0, Width = Dim.Percent(_uiSettings.Layout.LeftPanelWidth), Height = Dim.Fill(2)
        };

        _rightPanel = new FrameView($"Right: {_rightPath}")
        {
            X = Pos.Percent(_uiSettings.Layout.LeftPanelWidth), Y = 0, Width = Dim.Fill(), Height = Dim.Fill(2)
        };

        _leftList = new ListView()
        {
            X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(), CanFocus = true
        };

        _rightList = new ListView()
        {
            X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(), CanFocus = true
        };

        var footerText = $"{_appSettings.Title} v{_appSettings.Version} | {DateTime.Now.ToString(_uiSettings.StatusBar.DateTimeFormat)} | {_uiSettings.StatusBar.FunctionKeys}";
        _helpBar = new Label(footerText)
        {
            X = 0, Y = Pos.AnchorEnd(1), Width = Dim.Fill(), Height = 1
        };

        _timeBar = _helpBar; // Reference same label

        LoadDirectory(_leftPath, _leftList);
        LoadDirectory(_rightPath, _rightList);

        _leftPanel.Add(_leftList);
        _rightPanel.Add(_rightList);
        window.Add(_leftPanel, _rightPanel, _helpBar);

        SetActivePanel(0);
        UpdateTime();
        Application.Top.Add(window);
    }

    private void UpdateTime()
    {
        if (_uiSettings.StatusBar.ShowDateTime)
        {
            Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(1), (loop) =>
            {
                var footerText = $"{_appSettings.Title} v{_appSettings.Version} | {DateTime.Now.ToString(_uiSettings.StatusBar.DateTimeFormat)} | {_uiSettings.StatusBar.FunctionKeys}";
                _helpBar.Text = footerText;
                return true;
            });
        }
    }

    private void LoadDirectory(string path, ListView listView)
    {
        var items = new List<string>();
        
        try
        {
            if (Directory.GetParent(path) != null)
                items.Add("..");

            var dirs = Directory.GetDirectories(path)
                .Select(d => new { Name = $"[{Path.GetFileName(d)}]", Date = Directory.GetLastWriteTime(d) })
                .OrderBy(d => d.Name);

            var files = Directory.GetFiles(path)
                .Select(f => new { Name = Path.GetFileName(f), Date = File.GetLastWriteTime(f) })
                .OrderBy(f => f.Name);

            items.AddRange(dirs.Select(d => $"{d.Name.PadRight(_uiSettings.FilePanel.NameWidth)} {d.Date.ToString(_uiSettings.StatusBar.DateTimeFormat[..10])}"));
            items.AddRange(files.Select(f => $"{f.Name.PadRight(_uiSettings.FilePanel.NameWidth)} {f.Date.ToString(_uiSettings.StatusBar.DateTimeFormat[..10])}"));

            listView.SetSource(items);
        }
        catch
        {
            items.Add("Access Denied");
            listView.SetSource(items);
        }
    }

    public void NavigateInto()
    {
        var activeList = _activePanel == 0 ? _leftList : _rightList;
        var activePath = _activePanel == 0 ? _leftPath : _rightPath;
        
        if (activeList.SelectedItem >= 0 && activeList.Source.ToList().Count > activeList.SelectedItem)
        {
            var selected = activeList.Source.ToList()[activeList.SelectedItem].ToString();
            var fileName = selected?.Split(' ')[0];
            
            if (fileName == "..")
            {
                NavigateUp();
            }
            else if (fileName?.StartsWith("[") == true && fileName.EndsWith("]"))
            {
                var dirName = fileName[1..^1];
                var newPath = Path.Combine(activePath, dirName);
                NavigateToPath(newPath);
            }
        }
    }

    public void NavigateUp()
    {
        var activePath = _activePanel == 0 ? _leftPath : _rightPath;
        var parent = Directory.GetParent(activePath);
        
        if (parent != null)
            NavigateToPath(parent.FullName);
    }

    public void NavigateToPath(string path)
    {
        if (Directory.Exists(path))
        {
            if (_activePanel == 0)
            {
                _leftPath = path;
                _leftPanel.Title = $"Left: {_leftPath}";
                LoadDirectory(_leftPath, _leftList);
            }
            else
            {
                _rightPath = path;
                _rightPanel.Title = $"Right: {_rightPath}";
                LoadDirectory(_rightPath, _rightList);
            }
        }
    }

    public void SwitchPanel()
    {
        _activePanel = _activePanel == 0 ? 1 : 0;
        SetActivePanel(_activePanel);
    }

    private void SetActivePanel(int panel)
    {
        _leftPanel.ColorScheme = panel == 0 ? Colors.TopLevel : Colors.Base;
        _rightPanel.ColorScheme = panel == 1 ? Colors.TopLevel : Colors.Base;
        
        if (panel == 0)
            _leftList.SetFocus();
        else
            _rightList.SetFocus();
    }

    public string GetCurrentPath() => _activePanel == 0 ? _leftPath : _rightPath;
    
    public string GetSelectedItem()
    {
        var activeList = _activePanel == 0 ? _leftList : _rightList;
        var activePath = _activePanel == 0 ? _leftPath : _rightPath;
        
        if (activeList.SelectedItem >= 0 && activeList.Source.ToList().Count > activeList.SelectedItem)
        {
            var selected = activeList.Source.ToList()[activeList.SelectedItem].ToString();
            var fileName = selected?.Split(' ')[0];
            
            if (fileName != ".." && !string.IsNullOrEmpty(fileName))
            {
                var cleanName = fileName.StartsWith("[") && fileName.EndsWith("]") 
                    ? fileName[1..^1] : fileName;
                return Path.Combine(activePath, cleanName);
            }
        }
        return string.Empty;
    }

    public string GetInactivePanelPath() => _activePanel == 0 ? _rightPath : _leftPath;

    public void ShowSearch()
    {
        var searchDialog = new Dialog("Search Files", 60, 8);
        var searchLabel = new Label("Search for:") { X = 1, Y = 1 };
        var searchField = new TextField("") { X = 12, Y = 1, Width = Dim.Fill(1) };
        var searchButton = new Button("Search") { X = Pos.Center() - 5, Y = 3 };
        var cancelButton = new Button("Cancel") { X = Pos.Center() + 5, Y = 3 };

        string? searchTerm = null;
        searchButton.Clicked += () => { searchTerm = searchField.Text.ToString(); Application.RequestStop(); };
        cancelButton.Clicked += () => Application.RequestStop();

        searchDialog.Add(searchLabel, searchField, searchButton, cancelButton);
        Application.Run(searchDialog);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            SearchFiles(searchTerm);
        }
    }

    private void SearchFiles(string searchTerm)
    {
        var currentPath = GetCurrentPath();
        var results = new List<string>();
        
        try
        {
            var files = Directory.GetFiles(currentPath, $"*{searchTerm}*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(currentPath, f))
                .Take(100);
            
            var dirs = Directory.GetDirectories(currentPath, $"*{searchTerm}*", SearchOption.AllDirectories)
                .Select(d => $"[{Path.GetRelativePath(currentPath, d)}]")
                .Take(100);

            results.AddRange(dirs);
            results.AddRange(files);
        }
        catch (Exception ex)
        {
            results.Add($"Search error: {ex.Message}");
        }

        if (results.Count == 0)
            results.Add("No files found");

        var resultDialog = new Dialog($"Search Results for '{searchTerm}'", 80, 20);
        var resultList = new ListView(results) { X = 1, Y = 1, Width = Dim.Fill(1), Height = Dim.Fill(2) };
        var openButton = new Button("Open Location") { X = Pos.Center() - 10, Y = Pos.Bottom(resultList) + 1 };
        var closeButton = new Button("Close") { X = Pos.Center() + 5, Y = Pos.Bottom(resultList) + 1 };

        openButton.Clicked += () => 
        {
            if (resultList.SelectedItem >= 0 && results.Count > resultList.SelectedItem)
            {
                var selected = results[resultList.SelectedItem];
                if (!selected.StartsWith("Search error") && selected != "No files found")
                {
                    var cleanName = selected.Replace("[", "").Replace("]", "");
                    var fullPath = Path.Combine(currentPath, cleanName);
                    var directory = Directory.Exists(fullPath) ? fullPath : Path.GetDirectoryName(fullPath);
                    if (directory != null)
                        NavigateToPath(directory);
                }
            }
            Application.RequestStop();
        };
        
        closeButton.Clicked += () => Application.RequestStop();

        resultDialog.Add(resultList, openButton, closeButton);
        Application.Run(resultDialog);
    }

    public void ShowMessage(string title, string message)
    {
        MessageBox.Query(title, message, "OK");
    }

    public bool ShowConfirmation(string title, string message)
    {
        return MessageBox.Query(title, message, "Yes", "No") == 0;
    }

    public string? ShowDriveSelector(string[] drives)
    {
        if (drives.Length == 0) return null;
        
        var dialog = new Dialog("Select Drive", 50, 12);
        var list = new ListView(drives) { X = 1, Y = 1, Width = Dim.Fill(1), Height = Dim.Fill(2) };
        var ok = new Button("OK") { X = Pos.Center() - 5, Y = Pos.Bottom(list) + 1 };
        var cancel = new Button("Cancel") { X = Pos.Center() + 5, Y = Pos.Bottom(list) + 1 };

        string? selected = null;
        bool cancelled = false;
        ok.Clicked += () => { 
            if (list.SelectedItem >= 0 && list.SelectedItem < drives.Length)
                selected = drives[list.SelectedItem]; 
            Application.RequestStop(); 
        };
        cancel.Clicked += () => { cancelled = true; Application.RequestStop(); };

        dialog.Add(list, ok, cancel);
        Application.Run(dialog);
        return cancelled ? null : selected;
    }

    public void ShowHelp()
    {
        var helpText = @"ConSola File Manager - Complete Guide

═══════════════════════════════════════════════════════════════

NAVIGATION SHORTCUTS:
  ↑ ↓           Navigate up/down in file list
  Enter         Open directory or file
  Backspace     Go up one directory level (..)
  Tab           Switch between left/right panels
  Home          Go to first item in list
  End           Go to last item in list
  Page Up       Scroll up one page
  Page Down     Scroll down one page

FILE OPERATIONS:
  F5            Copy selected file/folder to other panel
  F6            Move selected file/folder to other panel
  F8 / Delete   Delete selected file/folder
  Ctrl+C        Copy to clipboard (future)
  Ctrl+V        Paste from clipboard (future)

SYSTEM OPERATIONS:
  F1            Show this help screen
  F3            Search files in current panel
  F10 / Esc     Exit ConSola (with confirmation)
  Ctrl+D        Change drive in active panel
  F9            Show file properties (future)
  Alt+F4        Force exit

DISPLAY FEATURES:
  • Active panel highlighted with different color
  • Directories shown with [square brackets]
  • File dates and times displayed
  • Real-time clock in status bar
  • Current path shown in panel headers

TIPS & TRICKS:
  • Use .. entry to navigate to parent directory
  • Tab quickly switches between panels
  • All operations show confirmation dialogs
  • File operations work on selected item
  • Drive selector shows available drives

═══════════════════════════════════════════════════════════════
ConSola v1.0 - Built with .NET 9 & Hexagonal Architecture";

        MessageBox.Query("ConSola Help & Shortcuts", helpText, "Close");
    }

    public void Shutdown()
    {
        Application.RequestStop();
    }
}