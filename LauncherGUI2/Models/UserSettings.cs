namespace LauncherGUI.Models;

/// <summary>
/// Пользовательские настройки интерфейса и локальных путей.
/// </summary>
public class UserSettings
{
    /// <summary>
    /// Индекс выбранной темы: 0 = темная, 1 = светлая.
    /// </summary>
    public int SelectedThemeIndex { get; set; }

    /// <summary>
    /// Пользовательский путь к папке установки.
    /// </summary>
    public string InstallationFolderPath { get; set; } = string.Empty;
}
