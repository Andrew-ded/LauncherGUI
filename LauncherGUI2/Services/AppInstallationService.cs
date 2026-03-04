using System;
using System.Threading;
using System.Threading.Tasks;

namespace LauncherGUI.Services;

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
