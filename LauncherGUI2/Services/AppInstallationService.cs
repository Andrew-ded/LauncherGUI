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


/// <summary>
/// Temporary installation service that simulates app setup.
/// </summary>
public class AppInstallationService : IAppInstallationService
{
    public async Task InstallAsync(IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        for (var value = 0; value <= 100; value += 4)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(value);
            await Task.Delay(100, cancellationToken);
        }
    }
}
