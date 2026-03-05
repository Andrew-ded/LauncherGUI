using Avalonia.Controls;
using Avalonia.Input;
using LauncherGUI.ViewModels;

namespace LauncherGUI.Views.Controls;

public partial class AppCardControl : UserControl
{
    public AppCardControl()
    {
        InitializeComponent();
    }

    private void AppCard_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not AppCardViewModel app || !app.Installed || !app.LaunchCommand.CanExecute(null))
        {
            return;
        }

        app.LaunchCommand.Execute(null);
    }
}
