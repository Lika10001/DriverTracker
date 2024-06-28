using System.Diagnostics;
namespace DriverTracker.Classes;

public class DriverManager(string exeFolderPath)
{
    private readonly Dictionary<string, Process> _runningProcesses = new();

    public void StartAllDrivers()
    {
        var files = Directory.GetFiles(exeFolderPath, "*.exe");
        Parallel.ForEach(files, StartDriver);
    }

    public void StopAllDrivers()
    {
        Parallel.ForEach(_runningProcesses.Values, StopDriver);
        _runningProcesses.Clear();
    }

    private void StartDriver(string filePath)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = Process.Start(startInfo);
            if (process != null) _runningProcesses.Add(Path.GetFileNameWithoutExtension(filePath), process);
            Console.WriteLine($"Started {Path.GetFileName(filePath)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting {Path.GetFileName(filePath)}: {ex.Message}");
        }
    }

    private void StopDriver(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill();
                Console.WriteLine($"Stopped {process.ProcessName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping {process.ProcessName}: {ex.Message}");
        }
    }

    public bool IsDriverRunning(string exeName)
    {
        if (_runningProcesses.TryGetValue(exeName, out var process))
            return !process.HasExited;
        return false;
    }
    
    public void StartDriverByName(string exeName)
    {
        string exeFilePath = Path.Combine(exeFolderPath, $"{exeName}.exe");
        if (File.Exists(exeFilePath))
        {
            if (!IsDriverRunning(exeName))
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = exeFilePath,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    var process = Process.Start(startInfo);
                    if (process != null) _runningProcesses.Add(exeName, process);
                    Console.WriteLine($"Started {exeName}.exe");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting {exeName}.exe: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"{exeName}.exe is already running.");
            }
        }
        else
        {
            Console.WriteLine($"File {exeName}.exe not found in the specified folder.");
        }
    }

    public void StopDriverByName(string exeName)
    {
        if (_runningProcesses.TryGetValue(exeName, out var process))
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    Console.WriteLine($"Stopped {exeName}.exe");
                }
                _runningProcesses.Remove(exeName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping {exeName}.exe: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"{exeName}.exe is not running.");
        }
    }
}