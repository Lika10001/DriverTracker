using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Classes;
using DriverTracker.Models;
using DriverTracker.Views;
using Device = DriverTracker.Models.Device;

namespace DriverTracker.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Device> _devices = new();
    [ObservableProperty] private ObservableCollection<Driver> _drivers = new();
    
    private readonly DriverManager _driverManager = new();
    private object _lockObject = new();
  
    private readonly AppDbContext _context = new ();

    [RelayCommand]
    private async Task StopAllDrivers()
    {
        await ExecuteAsync(() =>
        {
            var files = GetDriversToStart();
          
            try
            {
                _driverManager.StopAllDrivers(files);
                _driverManager.DeleteDriversFromDirectory();
            }
            catch (Exception)
            {
                throw new Exception();
            }

            return Task.CompletedTask;
        });
        CheckAllDriversForRunning();
        await Shell.Current.DisplayAlert("Drivers stopped", "Drivers for devices are successfully stopped.", "Ok");
    }

    [RelayCommand]
    private async Task GoToDetailsPage(Device device)
    {
        var currDeviceDriver = Drivers.FirstOrDefault(p => p.driver_id == device.device_driver_id);
        if (currDeviceDriver == null)
        {
            await Shell.Current.DisplayAlert("Details error", "No details found about this device", "Ok");
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(DeviceDetailsPage), true, new Dictionary<string, object>
            {
                {"Device", device },
                {"Driver", currDeviceDriver}
            });
        }
    }
    
    public async Task LoadDriversFromBd()
    {
        await ExecuteAsync(async () =>
        {
            var drivers = await _context.GetAllAsync<Driver>();
            var enumerable = drivers as Driver[] ?? drivers.ToArray();
            if (enumerable.Any())
            {
                Drivers.Clear();
                foreach (var driver in enumerable)
                {
                    Drivers.Add(driver);
                }
            }
        });
    }
    
    [RelayCommand]
    private async Task RunDriversForDevices()
    {
        _driverManager.LoadDriversToDirectory();
        await ExecuteAsync(() =>
        {
            var files = GetDriversToStart();
            try
            {
                _driverManager.StartAllDrivers(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.CompletedTask;
        });
        CheckAllDriversForRunning();
        await Shell.Current.DisplayAlert("Drivers are running", "Drivers for devices are successfully started.", "Ok");
    }

    private List<string> GetDriversToStart()
    {
        List<Device> devicesForUpdating = [..Devices];
        List<string> files = new();
        foreach (var device in devicesForUpdating)
        {
            var driver = Drivers.FirstOrDefault(predicate => predicate.driver_id == device.device_driver_id);
            if (driver != null) files.Add(driver.driver_name);
        }
        return files;
    }
    
    public async Task LoadDevicesAsync()
    {
        await ExecuteAsync(async () =>
        {
            var devices = await _context.GetAllAsync<Device>();
            var enumerable = devices as Device[] ?? devices.ToArray();
            if (enumerable.Any())
            {
                Devices.Clear();
                foreach (var device in enumerable)
                {
                    if (Devices.FirstOrDefault(p=>p.device_id == device.device_id) == null)
                    {
                        device.device_status = false;
                        Devices.Add(device);
                    }
                }
            }
        });
        //_driverManager.SaveDriverToDb("drv6", "0.0.0.6", 6);
    }
    
    private async Task UpdateDeviceStatusAsync(Device device)
    {
        await ExecuteAsync(async () =>
        {
            // Update device status
                if(await _context.UpdateItemAsync<Device>(device))
                {
                    var deviceCopy = device.Clone();
                    var oldDevice = Devices.FirstOrDefault(p=>p.device_id == device.device_id);
                    var index = Devices.IndexOf(oldDevice ?? throw new InvalidOperationException());
                    Devices.RemoveAt(index);
                    Devices.Insert(index, deviceCopy);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Device status update error", "Ok");
                    return;
                }
        });
    }
    
    private async Task ExecuteAsync(Func<Task> operation, string? busyText = null)
    {
        // IsBusy = true;
        //BusyText = busyText ?? "Processing...";
        try
        {
            await operation.Invoke();
        }
        catch(Exception)
        {
            throw new Exception();
        }
        finally
        {
            //IsBusy = false;
            //BusyText = "Processing...";
        }
    }

    public void CheckAllDriversForRunning()
    {
        var files = GetDriversToStart();
        try
        {
            Parallel.ForEach(files, CheckOneDeviceForRunning);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void CheckOneDeviceForRunning(string driverName)
    {
        try
        {
            var index = -1;
            var driver = Drivers.FirstOrDefault(driver => driver.driver_name == driverName);
           var currDevice = Devices.FirstOrDefault(p => driver != null && p.device_driver_id == driver.driver_id);
           if (currDevice != null)
           {
               lock(_lockObject){
                   index = Devices.IndexOf(currDevice);
                   Devices.RemoveAt(index);
                   var result = _driverManager.IsDriverRunning(driverName);
                   currDevice.device_status = result;
                   Devices.Insert(index, currDevice);
               }
           }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    [RelayCommand]
    private async Task NavigateToAddDeviceAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddDevicePage), true);
    }
    
    [RelayCommand]
    private async Task NavigateToSighInPageAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("///SighInPage", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
}