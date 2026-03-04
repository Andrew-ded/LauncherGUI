using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LauncherGUI.Models;

namespace LauncherGUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(LauncherConfig config)
    {
        Applications = new ObservableCollection<AppCardViewModel>(
            config.Applications.Select(app => new AppCardViewModel(app)));

        DeleteApplicationCommand = new RelayCommand(DeleteApplication);
        LaunchApplicationCommand = new RelayCommand(LaunchApplication);
    }

    public ObservableCollection<AppCardViewModel> Applications { get; }

    public RelayCommand DeleteApplicationCommand { get; }

    public RelayCommand LaunchApplicationCommand { get; }

    private void DeleteApplication(object? parameter)
    {
        if (parameter is AppCardViewModel app && Applications.Contains(app))
        {
            Applications.Remove(app);
        }
    }

    private static void LaunchApplication(object? parameter)
    {
        if (parameter is not AppCardViewModel app || string.IsNullOrWhiteSpace(app.ExecutablePath))
        {
            return;
        }

        var fileName = app.ExecutablePath;
        if (!Path.IsPathRooted(fileName) || File.Exists(fileName))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = true
            });
        }
    }
}
