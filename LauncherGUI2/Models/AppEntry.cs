



using Avalonia.Media.Imaging;

namespace LauncherGUI.Models;

public class AppEntry
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ExecutablePath { get; set; } = string.Empty;
    public string IconPath { get; set; } = string.Empty;

    public Bitmap IconApp { get; set; }
}
