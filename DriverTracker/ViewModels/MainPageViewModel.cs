using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DriverTracker.Classes;
using DriverTracker.Models;
using Device = DriverTracker.Models.Device;

namespace DriverTracker.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Device> _devices = new();
    [ObservableProperty] private ObservableCollection<Driver> _drivers = new();
  
    private readonly AppBDContext _context = new ();

    public MainPageViewModel()
    {
        LoadDevicesAsync();
        LoadDriversFromBD();
    }

    private static string GetCorrectPath()
    {
        var location = AppDomain.CurrentDomain.BaseDirectory;
        int indexOfEndingWord = location.LastIndexOf("DriverTracker");
        string substringToEndingWord = "";
        if (indexOfEndingWord != -1)
        {
            int lengthOfEndingWord = "DriverTracker".Length;
            substringToEndingWord = location.Substring(0, indexOfEndingWord + lengthOfEndingWord);
        }

        return Path.Combine(substringToEndingWord, @"Resources\Drivers");
    }
    
    public async Task LoadDriversFromBD()
    {
        await ExecuteAsync(async () =>
        {
            var drivers = await _context.GetAllAsync<Driver>();
            if (drivers is not null && drivers.Any())
            {
                _drivers ??= new ObservableCollection<Driver>();
                foreach (var driver in drivers)
                {
                    _drivers.Add(driver);
                }
            }
            await RunDriversForDevices();
        });
        
    }
    
    public async Task RunDriversForDevices()
    {
        await ExecuteAsync(async () =>
        {
            Device currDevice = new();
            string PathToDrivers = GetCorrectPath();
            DriverManager driverManager = new(PathToDrivers);
            foreach (var device in _devices)
            {
                try
                {
                    var driver = _drivers.FirstOrDefault(predicate => predicate.driver_id == device.device_driver_id);
                    if (driver != null)
                    {
                        driverManager.StartDriverByName(driver.driver_name);
                        currDevice.device_status = 1;
                    }
                    else
                    {
                        currDevice.device_status = 0;
                      
                    }

                    currDevice.device_driver_id = device.device_driver_id;
                    currDevice.device_id = device.device_id;
                    currDevice.device_name = device.device_name;
                    await UpdateDeviceStatusAsync(currDevice);
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
        });
    }
    
    
    
    public async Task LoadDevicesAsync()
    {
        await ExecuteAsync(async () =>
        {
            var devices = await _context.GetAllAsync<Device>();
            if (devices is not null && devices.Any())
            {
                _devices ??= new ObservableCollection<Device>();
                foreach (var device in devices)
                {
                    _devices.Add(device);
                }
            }
        });
    }
    
    private async Task UpdateDeviceStatusAsync(Device device)
    {
        await ExecuteAsync(async () =>
        {
            // Update device status
                if(await _context.UpdateItemAsync<Device>(device))
                {
                    var deviceCopy = device.Clone();

                    var index = _devices.IndexOf(device);
                    _devices.RemoveAt(index);

                    _devices.Insert(index, deviceCopy);
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
            await operation?.Invoke();
        }
        catch(Exception ex)
        {
                
        }
        finally
        {
            //IsBusy = false;
            //BusyText = "Processing...";
        }
    }
    
}