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
  
    private readonly AppBDContext _context = new ();

    public MainPageViewModel()
    {
        _ = LoadDevicesAsync();
        _ = LoadDriversFromBD();
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

    [RelayCommand]
    public async Task StopAllDrivers()
    {
       
            Device currDevice = new();
            DriverManager driverManager = new(GetCorrectPath());
            foreach (var device in _devices)
            {
                if (device.device_status == 1)
                {
                    try
                    {
                        driverManager.StopDriverByName(
                            (_drivers.FirstOrDefault(p => p.driver_id == device.device_driver_id)).driver_name);
                        currDevice.device_id = device.device_id;
                        currDevice.device_name = device.device_name;
                        currDevice.device_status = 0;
                        currDevice.device_driver_id = device.device_driver_id;
                        await UpdateDeviceStatusAsync(currDevice);
                    }
                    catch (Exception)
                    {
                        throw new Exception();
                    }
                    
                }
            }    
    }

    [RelayCommand]
    public async Task GoToDetailsPage(Device device)
    {
        Driver currDeviceDriver = new Driver();
        currDeviceDriver = _drivers.FirstOrDefault(p => p.driver_id == device.device_driver_id);
        if (device == null || currDeviceDriver == null)
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
    [RelayCommand]
    public async Task RunDriversForDevices()
    {
        await ExecuteAsync(async () =>
        {
            Device currDevice = new();
            string PathToDrivers = GetCorrectPath();
            DriverManager driverManager = new(PathToDrivers);
            foreach (var device in _devices)
            {
                if (device.device_status == 0){
                    try
                    {
                        var driver =
                            _drivers.FirstOrDefault(predicate => predicate.driver_id == device.device_driver_id);
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

                    var index = Devices.IndexOf(device);
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

    [RelayCommand]
    public async Task NavigateToAddDeviceAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddDevicePage), true);
    }
    
}