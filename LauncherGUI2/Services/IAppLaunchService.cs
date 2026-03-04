using System.Threading;
using System.Threading.Tasks;

namespace LauncherGUI.Services;

/// <summary>
/// Service responsible for launching processes and piping their output.
/// </summary>
public interface IAppLaunchService
{
    Task LaunchAndPipeOutputAsync(string executablePath, CancellationToken cancellationToken = default);
}
