using System;
using Avalonia;

namespace LauncherGUI;

/// <summary>
/// Точка входа в приложение и базовая настройка Avalonia.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Запускает приложение в классическом desktop режиме.
    /// </summary>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Создает и настраивает <see cref="AppBuilder"/> для runtime и дизайнера.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
