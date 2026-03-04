using LauncherGUI.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using System.IO;
using System.Text.Json;
using System.Drawing.Imaging;

namespace LauncherGUI.Services;

/// <summary>
/// Загружает конфигурацию лаунчера и подготавливает иконки приложений для Avalonia UI.
/// </summary>
public class AppConfigService
{
    /// <summary>Имя файла с настройками приложений.</summary>
    private const string ConfigFileName = "appsettings.json";

    /// <summary>
    /// Загружает конфигурацию из <c>appsettings.json</c>.
    /// Если файл отсутствует или пустой, возвращает пустую конфигурацию.
    /// </summary>
    public LauncherConfig Load()
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);

        if (!File.Exists(fullPath))
        {
            return new LauncherConfig();
        }

        var json = File.ReadAllText(fullPath);
        var config = JsonSerializer.Deserialize<LauncherConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new LauncherConfig();

        // Для каждой записи пытаемся извлечь системную иконку EXE и перевести ее в формат Avalonia.
        foreach (var app in config.Applications)
        {
            if (File.Exists(app.ExecutablePath))
            {
                var icon = Icon.ExtractAssociatedIcon(app.ExecutablePath);
                if (icon is null)
                {
                    continue;
                }

                var winBitmap = icon.ToBitmap();
                using var ms = new MemoryStream();
                winBitmap.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                app.IconApp = new AvaloniaBitmap(ms);
            }
            else
            {
                Debug.WriteLine("Не найден " + app.ExecutablePath);
            }

            // При необходимости можно вернуть нормализацию iconPath в UI-совместимый avares URI.
            // app.IconPath = NormalizeIconPath(app.IconPath);
        }

        return config;
    }

    /// <summary>
    /// Нормализует путь к иконке в формат avares://LauncherGUI2/Assets/Icons/*. 
    /// Метод пока не используется, оставлен как утилита.
    /// </summary>
    private static string NormalizeIconPath(string iconPath)
    {
        if (string.IsNullOrWhiteSpace(iconPath))
        {
            return "avares://LauncherGUI2/Assets/Icons/apps.png";
        }

        var normalized = iconPath.Trim().Replace("\\", "/");

        if (normalized.StartsWith("avares://LauncherGUI/", StringComparison.OrdinalIgnoreCase))
        {
            normalized = "avares://LauncherGUI2/" + normalized[21..];
        }

        if (normalized.StartsWith("avares://", StringComparison.OrdinalIgnoreCase))
        {
            return normalized;
        }

        var fileName = Path.GetFileName(normalized);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return "avares://LauncherGUI2/Assets/Icons/apps.png";
        }

        return $"avares://LauncherGUI2/Assets/Icons/{fileName}";
    }
}
