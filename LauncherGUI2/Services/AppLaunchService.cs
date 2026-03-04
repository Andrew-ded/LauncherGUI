using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LauncherGUI.Services;

/// <summary>
/// Launches external process and forwards stdout/stderr to IDE console.
/// </summary>
public class AppLaunchService : IAppLaunchService
{
    public async Task LaunchAndPipeOutputAsync(string executablePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(executablePath))
        {
            return;
        }

        if (Path.IsPathRooted(executablePath) && !File.Exists(executablePath))
        {
            Debug.WriteLine($"[launcher] File not found: {executablePath}");
            Console.WriteLine($"[launcher] File not found: {executablePath}");
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            var line = $"[stdout] {args.Data}";
            Debug.WriteLine(line);
            Console.WriteLine(line);
        };

        process.ErrorDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            var line = $"[stderr] {args.Data}";
            Debug.WriteLine(line);
            Console.WriteLine(line);
        };

        if (!process.Start())
        {
            return;
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        var exitLine = $"[launcher] Exit code: {process.ExitCode}";
        Debug.WriteLine(exitLine);
        Console.WriteLine(exitLine);
    }
}
