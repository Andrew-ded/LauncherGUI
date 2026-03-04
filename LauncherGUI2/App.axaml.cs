using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LauncherGUI.Services;
using LauncherGUI.ViewModels;
using LauncherGUI.Views;

namespace LauncherGUI;

/// <summary>
/// Root Avalonia application object.
/// </summary>
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var configService = new AppConfigService();
            var installationService = new AppInstallationService();
            var launchService = new AppLaunchService();
            var userSettingsService = new UserSettingsService();
            var userSettings = userSettingsService.Load();

            var viewModel = new MainWindowViewModel(
                configService.Load(),
                installationService,
                launchService,
                userSettings,
                userSettingsService);

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
