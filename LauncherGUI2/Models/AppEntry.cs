using Avalonia.Media.Imaging;

namespace LauncherGUI.Models;

/// <summary>
/// Configuration model for one launcher application entry.
/// </summary>
public class AppEntry
{
    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Display version.</summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>Absolute path to executable file.</summary>
    public string ExecutablePath { get; set; } = string.Empty;

    /// <summary>Optional icon path from configuration.</summary>
    public string IconPath { get; set; } = string.Empty;

    /// <summary>
    /// True when the app can be installed by the user (shown on install page).
    /// </summary>
    public bool Available { get; set; }

    /// <summary>
    /// True when the app is already installed (shown on main page).
    /// </summary>
    public bool Installed { get; set; }

    /// <summary>
    /// Prepared Avalonia bitmap used by UI cards.
    /// </summary>
    public Bitmap IconApp { get; set; } = null!;
}
