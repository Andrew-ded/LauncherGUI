
using Avalonia.Media.Imaging;
using LauncherGUI.Models;


namespace LauncherGUI.ViewModels;

public class AppCardViewModel : ViewModelBase
{
    public AppCardViewModel(AppEntry model)
    {
        Name = model.Name;
        Version = model.Version;
        ExecutablePath = model.ExecutablePath;
        IconPath = model.IconPath;
        IconApp = model.IconApp;
    }

    public string Name { get; }
    public string Version { get; }
    public string ExecutablePath { get; }
    public string IconPath { get; }

    public Bitmap IconApp { get;}
}
