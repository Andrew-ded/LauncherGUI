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
        if (DataContext is not AppCardViewModel app || !app.Installed)
        {
            return;
        }

        if (VisualRoot is Window { DataContext: MainWindowViewModel vm } && vm.LaunchApplicationCommand.CanExecute(app))
        {
            vm.LaunchApplicationCommand.Execute(app);
        }
    }
}
