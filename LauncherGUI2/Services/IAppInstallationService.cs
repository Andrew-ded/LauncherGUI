using System;
using System.Threading;
using System.Threading.Tasks;

namespace LauncherGUI.Services;

/// <summary>
/// Service responsible for application installation workflow.
/// </summary>
public interface IAppInstallationService
{
    /// <summary>
    /// Runs installation process and reports progress in range [0..100].
    /// </summary>
    Task InstallAsync(IProgress<int>? progress = null, CancellationToken cancellationToken = default);
}
