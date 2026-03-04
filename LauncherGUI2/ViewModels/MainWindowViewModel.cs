using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using LauncherGUI.Models;
using LauncherGUI.Services;

namespace LauncherGUI.ViewModels;

/// <summary>
/// Main window view model: section state, app collections, settings and commands.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    private const string ApplicationsSection = "applications";
    private const string SecuritySection = "security";
    private const string SettingsSection = "settings";

    private string _selectedSection = ApplicationsSection;
    private readonly IAppInstallationService _installationService;
    private readonly IAppLaunchService _launchService;
    private readonly IUserSettingsService _userSettingsService;
    private readonly UserSettings _userSettings;

    // Настройки экрана Settings.
    private int _selectedThemeIndex;
    private string _installationFolderPath = string.Empty;

    public MainWindowViewModel(
        LauncherConfig config,
        IAppInstallationService installationService,
        IAppLaunchService launchService,
        UserSettings userSettings,
        IUserSettingsService userSettingsService)
    {
        _installationService = installationService;
        _launchService = launchService;
        _userSettings = userSettings;
        _userSettingsService = userSettingsService;

        var allCards = config.Applications
            .Select(app => new AppCardViewModel(app))
            .ToList();

        InstalledApplications = new ObservableCollection<AppCardViewModel>(
            allCards.Where(app => app.Installed));

        CatalogApplications = new ObservableCollection<AppCardViewModel>(
            allCards.Where(app => !app.Installed));

        DeleteApplicationCommand = new RelayCommand(DeleteApplication);
        LaunchApplicationCommand = new RelayCommand(LaunchApplication);
        SwitchSectionCommand = new RelayCommand(SwitchSection);
        InstallApplicationCommand = new RelayCommand(InstallApplication);

        // Источник для ComboBox выбора темы.
        ThemeOptions = ["Темная", "Светлая"];
        _selectedThemeIndex = NormalizeThemeIndex(_userSettings.SelectedThemeIndex);
        _installationFolderPath = _userSettings.InstallationFolderPath ?? string.Empty;

        // Применяем сохраненную тему при запуске.
        ApplyTheme();
    }

    /// <summary>Apps already installed and available for launch.</summary>
    public ObservableCollection<AppCardViewModel> InstalledApplications { get; }

    /// <summary>Apps shown on installation page (both available and unavailable).</summary>
    public ObservableCollection<AppCardViewModel> CatalogApplications { get; }

    public RelayCommand DeleteApplicationCommand { get; }
    public RelayCommand LaunchApplicationCommand { get; }
    public RelayCommand SwitchSectionCommand { get; }
    public RelayCommand InstallApplicationCommand { get; }

    /// <summary>
    /// Варианты темы для выпадающего списка.
    /// </summary>
    public ObservableCollection<string> ThemeOptions { get; }

    /// <summary>
    /// Выбранная тема: 0 = темная, 1 = светлая.
    /// При изменении сразу применяется ко всему приложению.
    /// </summary>
    public int SelectedThemeIndex
    {
        get => _selectedThemeIndex;
        set
        {
            if (_selectedThemeIndex == value)
            {
                return;
            }

            _selectedThemeIndex = value;
            RaisePropertyChanged();
            ApplyTheme();
            SaveUserSettings();
        }
    }

    /// <summary>
    /// Путь к папке, который задается в настройках.
    /// </summary>
    public string InstallationFolderPath
    {
        get => _installationFolderPath;
        set
        {
            if (_installationFolderPath == value)
            {
                return;
            }

            _installationFolderPath = value;
            RaisePropertyChanged();
            SaveUserSettings();
        }
    }

    // Selected section state.
    public bool IsApplicationsSectionSelected => _selectedSection == ApplicationsSection;
    public bool IsSecuritySectionSelected => _selectedSection == SecuritySection;
    public bool IsSettingsSectionSelected => _selectedSection == SettingsSection;

    // Section visibility flags for XAML binding.
    public bool IsApplicationsSectionVisible => IsApplicationsSectionSelected;
    public bool IsSecuritySectionVisible => IsSecuritySectionSelected;
    public bool IsSettingsSectionVisible => IsSettingsSectionSelected;

    private void DeleteApplication(object? parameter)
    {
        if (parameter is AppCardViewModel app && InstalledApplications.Contains(app))
        {
            InstalledApplications.Remove(app);
            app.Installed = false;

            if (!CatalogApplications.Contains(app))
            {
                CatalogApplications.Add(app);
            }
        }
    }

    private void SwitchSection(object? parameter)
    {
        if (parameter is not string section)
        {
            return;
        }

        section = section.ToLowerInvariant();
        if (section != ApplicationsSection && section != SecuritySection && section != SettingsSection)
        {
            return;
        }

        if (_selectedSection == section)
        {
            return;
        }

        _selectedSection = section;

        RaisePropertyChanged(nameof(IsApplicationsSectionSelected));
        RaisePropertyChanged(nameof(IsSecuritySectionSelected));
        RaisePropertyChanged(nameof(IsSettingsSectionSelected));
        RaisePropertyChanged(nameof(IsApplicationsSectionVisible));
        RaisePropertyChanged(nameof(IsSecuritySectionVisible));
        RaisePropertyChanged(nameof(IsSettingsSectionVisible));
    }

    private async void InstallApplication(object? parameter)
    {
        if (parameter is not AppCardViewModel app || !CatalogApplications.Contains(app) || !app.IsInstallable || app.IsInstalling)
        {
            return;
        }

        app.IsInstalling = true;
        app.InstallProgress = 0;
        app.SpinnerAngle = 0;

        var progress = new Progress<int>(value =>
        {
            app.InstallProgress = value;
            app.SpinnerAngle = (app.SpinnerAngle + 20) % 360;
        });

        await _installationService.InstallAsync(progress);

        // Keep card visible without blur for a second after install reaches 100%.
        app.IsInstalling = false;
        await Task.Delay(1000);

        app.Installed = true;
        if (CatalogApplications.Contains(app))
        {
            CatalogApplications.Remove(app);
        }

        if (!InstalledApplications.Contains(app))
        {
            InstalledApplications.Add(app);
        }
    }

    private async void LaunchApplication(object? parameter)
    {
        if (parameter is not AppCardViewModel app || string.IsNullOrWhiteSpace(app.ExecutablePath))
        {
            return;
        }

        await _launchService.LaunchAndPipeOutputAsync(app.ExecutablePath);
    }

    /// <summary>
    /// Применяет выбранную тему к текущему приложению.
    /// </summary>
    private void ApplyTheme()
    {
        if (Application.Current is null)
        {
            return;
        }

        Application.Current.RequestedThemeVariant =
            SelectedThemeIndex == 1 ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    /// <summary>
    /// Приводит индекс темы к допустимому диапазону [0..1].
    /// </summary>
    private static int NormalizeThemeIndex(int value)
    {
        return value == 1 ? 1 : 0;
    }

    /// <summary>
    /// Сохраняет измененные пользовательские настройки в отдельный JSON-файл.
    /// </summary>
    private void SaveUserSettings()
    {
        _userSettings.SelectedThemeIndex = SelectedThemeIndex;
        _userSettings.InstallationFolderPath = InstallationFolderPath;
        _userSettingsService.Save(_userSettings);
    }
}
