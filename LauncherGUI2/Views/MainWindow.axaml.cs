using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using LauncherGUI.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LauncherGUI.Views;

/// <summary>
/// Код главного окна: обработчики действий интерфейса,
/// управление системными кнопками и открытие диалогов.
/// </summary>
public partial class MainWindow : Window
{
    // Флаг защищает от повторного запуска анимации, пока текущая еще идет.
    private bool _isSyncAnimationRunning;

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Позволяет перетаскивать окно за кастомную область заголовка.
    /// </summary>
    private void TitleBar_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    /// <summary>
    /// Сворачивает окно в панель задач.
    /// </summary>
    private void Minimize_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Переключает состояние окна между обычным и развернутым.
    /// </summary>
    private void ToggleMaximize_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    /// <summary>
    /// Закрывает приложение.
    /// </summary>
    private void Close_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Открывает системный диалог выбора папки и сохраняет путь в настройках VM.
    /// </summary>
    private async void BrowseFolder_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Выберите папку установки",
            AllowMultiple = false
        });

        var picked = folders.FirstOrDefault();
        if (picked is null)
        {
            return;
        }

        vm.InstallationFolderPath = picked.Path.LocalPath;
    }

    /// <summary>
    /// Имитация процесса сборки JSON-каталога:
    /// в течение одной секунды вращаем иконку кнопки синхронизации.
    /// </summary>
    private async void SyncCatalog_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isSyncAnimationRunning)
        {
            return;
        }

        if (this.FindControl<PathIcon>("SyncCatalogIcon") is not { } icon)
        {
            return;
        }

        _isSyncAnimationRunning = true;
        const int totalDurationMs = 1000;
        const int totalFrames = 60;
        var frameDelayMs = Math.Max(1, totalDurationMs / totalFrames);

        try
        {
            for (var frame = 0; frame <= totalFrames; frame++)
            {
                var angle = 360.0 * frame / totalFrames;
                icon.RenderTransform = new RotateTransform(angle);
                await Task.Delay(frameDelayMs);
            }
        }
        finally
        {
            icon.RenderTransform = new RotateTransform(0);
            _isSyncAnimationRunning = false;
        }
    }
}
