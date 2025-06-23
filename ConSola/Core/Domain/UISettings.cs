namespace ConSola.Core.Domain;

public class UISettings
{
    public LayoutSettings Layout { get; set; } = new();
    public FilePanelSettings FilePanel { get; set; } = new();
    public StatusBarSettings StatusBar { get; set; } = new();
}

public class LayoutSettings
{
    public int LeftPanelWidth { get; set; } = 50;
    public int RightPanelWidth { get; set; } = 50;
    public int StatusBarHeight { get; set; } = 1;
}

public class FilePanelSettings
{
    public int NameWidth { get; set; } = 30;
    public int SizeWidth { get; set; } = 12;
    public int DateWidth { get; set; } = 16;
}

public class StatusBarSettings
{
    public bool ShowDateTime { get; set; } = true;
    public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
    public string FunctionKeys { get; set; } = "F1=Help F3=Search F5=Copy F6=Move F8=Delete F10=Quit";
}

public class ApplicationSettings
{
    public string Version { get; set; } = "1.0.0";
    public string Title { get; set; } = "ConSola";
}