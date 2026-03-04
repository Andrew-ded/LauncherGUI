using Avalonia.Media.Imaging;
using LauncherGUI.Models;

namespace LauncherGUI.ViewModels;

/// <summary>
/// UI view model for one application card.
/// </summary>
public class AppCardViewModel : ViewModelBase
{
    private bool _installed;
    private bool _available;
    private bool _isInstalling;
    private int _installProgress;
    private double _spinnerAngle;

    public AppCardViewModel(AppEntry model)
    {
        Name = model.Name;
        Version = model.Version;
        ExecutablePath = model.ExecutablePath;
        IconPath = model.IconPath;
        IconApp = model.IconApp;

        _available = model.Available;
        _installed = model.Installed;
    }

    public string Name { get; }
    public string Version { get; }
    public string ExecutablePath { get; }
    public string IconPath { get; }
    public Bitmap IconApp { get; }

    public bool Installed
    {
        get => _installed;
        set
        {
            if (_installed == value) return;
            _installed = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsInstallable));
            RaisePropertyChanged(nameof(IsUnavailable));
            RaisePropertyChanged(nameof(IsNotInstalled));
            RaisePropertyChanged(nameof(IsInstallButtonVisible));
            RaisePropertyChanged(nameof(IsLockOverlayVisible));
        }
    }

    public bool Available
    {
        get => _available;
        set
        {
            if (_available == value) return;
            _available = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsInstallable));
            RaisePropertyChanged(nameof(IsUnavailable));
            RaisePropertyChanged(nameof(IsInstallButtonVisible));
            RaisePropertyChanged(nameof(IsLockOverlayVisible));
        }
    }

    public bool IsInstalling
    {
        get => _isInstalling;
        set
        {
            if (_isInstalling == value) return;
            _isInstalling = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsInstallButtonEnabled));
            RaisePropertyChanged(nameof(IsInstallingOverlayVisible));
        }
    }

    public int InstallProgress
    {
        get => _installProgress;
        set
        {
            if (_installProgress == value) return;
            _installProgress = value;
            RaisePropertyChanged();
        }
    }

    public bool IsInstallButtonEnabled => !IsInstalling;

    public bool IsInstallingOverlayVisible => IsInstalling;

    public bool IsInstallable => Available && !Installed;

    public bool IsUnavailable => !Available && !Installed;

    public bool IsNotInstalled => !Installed;

    public bool IsInstallButtonVisible => IsInstallable;

    public bool IsLockOverlayVisible => IsUnavailable;

    public double SpinnerAngle
    {
        get => _spinnerAngle;
        set
        {
            if (System.Math.Abs(_spinnerAngle - value) < 0.001) return;
            _spinnerAngle = value;
            RaisePropertyChanged();
        }
    }
}
