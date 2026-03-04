using LauncherGUI.Models;

namespace LauncherGUI.Services;

/// <summary>
/// Сервис чтения и сохранения пользовательских настроек.
/// </summary>
public interface IUserSettingsService
{
    UserSettings Load();
    void Save(UserSettings settings);
}
