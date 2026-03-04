using System.Collections.Generic;

namespace LauncherGUI.Models;

/// <summary>
/// Корневая модель конфигурации лаунчера.
/// </summary>
public class LauncherConfig
{
    /// <summary>
    /// Список приложений, отображаемых в интерфейсе.
    /// </summary>
    public List<AppEntry> Applications { get; set; } = [];
}
