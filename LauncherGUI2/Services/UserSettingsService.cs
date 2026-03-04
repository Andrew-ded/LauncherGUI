using System;
using System.IO;
using System.Text.Json;
using LauncherGUI.Models;

namespace LauncherGUI.Services;

/// <summary>
/// Реализация сервиса пользовательских настроек в отдельный JSON-файл.
/// </summary>
public class UserSettingsService : IUserSettingsService
{
    private const string UserSettingsFileName = "user-settings.json";

    /// <summary>
    /// Загружает настройки пользователя из user-settings.json.
    /// Если файл отсутствует или поврежден, возвращает настройки по умолчанию.
    /// </summary>
    public UserSettings Load()
    {
        var path = GetSettingsPath();
        if (!File.Exists(path))
        {
            return new UserSettings();
        }

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<UserSettings>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new UserSettings();
        }
        catch
        {
            return new UserSettings();
        }
    }

    /// <summary>
    /// Сохраняет настройки пользователя в user-settings.json.
    /// </summary>
    public void Save(UserSettings settings)
    {
        var path = GetSettingsPath();
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(path, json);
    }

    private static string GetSettingsPath()
    {
        return Path.Combine(AppContext.BaseDirectory, UserSettingsFileName);
    }
}
