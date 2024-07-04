using System.Diagnostics;
using System.Text;
using DriverTracker.Models;
using Microsoft.Data.Sqlite;
using SQLite;

namespace DriverTracker.Classes;

public class DriverManager
{
    private readonly Dictionary<string, Process> _runningProcesses = new();
    private readonly string _driversPath = GetCorrectPath();
    private readonly object _startDriverLock = new();
    private List<Task> _tasks = new ();
    
    
   public async void StartAllDrivers(List<string> files)
    {
        //var files = Directory.GetFiles(_driversPath, "*.exe");
        try
        {
            Parallel.ForEach(files, StartDriverByName);
            await Task.WhenAll(_tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void StopAllDrivers(List<string> files)
    {
       // var files = Directory.GetFiles(_driversPath, "*.exe");
       try
       {
           Parallel.ForEach(files, StopDriverByName);
           lock (_startDriverLock)
           {
               _runningProcesses.Clear();
           }
       }
       catch (Exception ex)
       {
           Console.WriteLine(ex.Message);
       }
    }
    
    public bool IsDriverRunning(string exeName)
    {
        bool isRunning = false;
        var processes = Process.GetProcesses();
        Parallel.ForEach(processes, process =>
        {
            try
            {
                if (!process.HasExited && process.ProcessName.Equals(exeName, StringComparison.OrdinalIgnoreCase))
                {
                    isRunning = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        });
        
        return isRunning;
        
    }

    public void SaveDriverToDb(string driverName, string ip, int port)
    {
        const String databaseFileName = "DriverTrackerDB.db";
        string projectDirectory = "C:\\Users\\User\\RiderProjects";
    
        string dbPath = Path.Combine(projectDirectory, "DriverTracker", "DriverTracker", "Resources", "Database", databaseFileName);
        string exeFilePath = Path.Combine(_driversPath, $"{driverName}.exe");
        byte[] binFileData;
        using (FileStream fs = new FileStream(exeFilePath, FileMode.Open))
        {
            binFileData = new byte[fs.Length];
            fs.Read(binFileData, 0, binFileData.Length);
        }

        try
        {
            using var connection = new SqliteConnection($"Data Source = {dbPath}");
            connection.Open();
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            command.CommandText = @"INSERT INTO Driver (driver_name, driver_ip, driver_port, driver_file)
                                        VALUES (@driver_name, @driver_ip, @driver_port, @driver_file)";
            command.Parameters.Add(new SqliteParameter("@driver_name", driverName));
            command.Parameters.Add(new SqliteParameter("@driver_ip", ip));
            command.Parameters.Add(new SqliteParameter("@driver_port", port));
            command.Parameters.Add(new SqliteParameter("@driver_file", binFileData));
            int number = command.ExecuteNonQuery();
            Console.WriteLine($"Inserted objects: {number}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    public void LoadDriversToDirectory()
    {
        List<Driver> drivers = new List<Driver>();
        string sql = "SELECT * FROM Driver";
        const string databaseFileName = "DriverTrackerDB.db";
        const string projectDirectory = "C:\\Users\\User\\RiderProjects";
    
        string dbPath = Path.Combine(projectDirectory, "DriverTracker", "DriverTracker", "Resources", "Database", databaseFileName) ?? throw new ArgumentNullException();
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
 
        SqliteCommand command = new SqliteCommand(sql, connection);
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows) 
            {
                while (reader.Read()) 
                {
                    int id = reader.GetInt32(0);
                    string driverName = reader.GetString(1);
                    string driverIp = reader.GetString(2);
                    int driverPort = reader.GetInt32(3);
                    byte[] driverFile = (byte[])reader.GetValue(4);
                    Driver driver = new (id, driverName, driverIp, driverPort, driverFile);
                    drivers.Add(driver);
                }
            }
            Console.WriteLine($"Drivers count: {drivers.Count}");
        }
            
        if (drivers.Count > 0)
        {
        
            foreach (var driver in drivers)
            {
                if (!IsDriverRunning(driver.driver_name) || !File.Exists(Path.Combine(_driversPath, $"{driver.driver_name}.exe")))
                {
                    using FileStream fs = new FileStream(Path.Combine(_driversPath, $"{driver.driver_name}.exe"),
                        FileMode.OpenOrCreate);
                    fs.Write(driver.driver_file, 0, driver.driver_file.Length);
                    Console.WriteLine($"File {driver.driver_name} saved");
                }
            }
        }
    }

    public void DeleteDriversFromDirectory()
    {
        DirectoryInfo di = new DirectoryInfo(_driversPath);
        foreach (FileInfo file in di.GetFiles())
        {
            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void StartDriverByName(string exeName)
    {
            string exeFilePath = Path.Combine(_driversPath, $"{exeName}.exe");
            if (File.Exists(exeFilePath))
            {
                if (!IsDriverRunning(exeName))
                {
                    try
                    {
                        //log file name = name of exe + _log.exe
                        //thread name = name of exe +_writer
                        
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = exeFilePath,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };
                        var process = Process.Start(startInfo);
                        string logFilePath = Path.Combine(_driversPath, "Logs", exeName + "_log.txt");
                        if (process != null)
                        {
                            try
                            {
                                using FileStream fileStream = new FileStream(logFilePath, FileMode.Append,
                                    FileAccess.Write, FileShare.Read);
                                StreamPipe pout = new StreamPipe(process.StandardOutput.BaseStream,
                                    fileStream);
                                StreamPipe perr = new StreamPipe(process.StandardError.BaseStream,
                                    fileStream);
                                pout.Connect();
                                perr.Connect();
                            
                                _tasks.Add(Task.Run(() =>
                                {
                                    process.WaitForExit();
                                    pout.Disconnect();
                                    perr.Disconnect();
                                    Console.WriteLine($"Process exited with code: {process.ExitCode}");
                                    Console.WriteLine($"Standard output and error written to: {logFilePath}");
                                }));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                           
                        }

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
        while (IsDriverRunning(exeName))
        {
            try
            {
                Process? process = null;
                object foundProcessLock = new object();
                Process[] processes = Process.GetProcesses();
                Parallel.ForEach(processes, p =>
                {
                    try
                    {
                        if (!p.HasExited && p.ProcessName.Equals(exeName, StringComparison.OrdinalIgnoreCase))
                        {
                            lock (foundProcessLock)
                            {
                                process = p;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                if (process != null)
                {
                    process.Kill();
                    process.WaitForExit();
                }

                if (process != null) process.Dispose();
                Console.WriteLine($"Stopped {exeName}.exe");
                if (_runningProcesses.ContainsKey(exeName)) _runningProcesses.Remove(exeName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping {exeName}.exe: {ex.Message}");
            }
        }

        Console.WriteLine($"{exeName}.exe is not running.");
        
    }
   
    
    private static string GetCorrectPath()
    {
        var location = AppDomain.CurrentDomain.BaseDirectory;
        int indexOfEndingWord = location.LastIndexOf("DriverTracker", StringComparison.Ordinal);
        string substringToEndingWord = "";
        if (indexOfEndingWord != -1)
        {
            int lengthOfEndingWord = "DriverTracker".Length;
            substringToEndingWord = location.Substring(0, indexOfEndingWord + lengthOfEndingWord);
        }

        return Path.Combine(substringToEndingWord, @"Resources\Drivers");
    }

    class StreamPipe
    {
        private const Int32 BufferSize = 4096;

        private Stream Source { get; set; }
        private Stream Destination { get; set; }

        private CancellationTokenSource _cancellationToken;
        private Task _worker;

        public StreamPipe(Stream source, Stream destination)
        {
            Source = source;
            Destination = destination;
        }

        public StreamPipe Connect()
        {
            _cancellationToken = new CancellationTokenSource();
            _worker = Task.Run(async () =>
            {
                byte[] buffer = new byte[BufferSize];
                while (true)
                {
                    _cancellationToken.Token.ThrowIfCancellationRequested();
                    var count = await Source.ReadAsync(buffer, 0, BufferSize, _cancellationToken.Token);
                    if (count <= 0)
                        break;
                    await Destination.WriteAsync(buffer, 0, count, _cancellationToken.Token);
                    await Destination.FlushAsync(_cancellationToken.Token);
                }
            }, _cancellationToken.Token);
            return this;
        }

        public void Disconnect()
        {
            _cancellationToken.Cancel();
        }
    }
}