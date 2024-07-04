using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Classes;
using DriverTracker.Views;
using DriverTracker.Models;
using Device = DriverTracker.Models.Device;

namespace DriverTracker.ViewModels;

[QueryProperty(nameof(Models.Device), "Device")]
[QueryProperty(nameof(Models.Driver), "Driver")]
public partial class DeviceDetailsViewModel:ObservableObject
{
    [ObservableProperty] private Device _device = new();
    [ObservableProperty] private Driver _driver = new();

    [ObservableProperty] private ObservableCollection<Device> _devices = new();
    [ObservableProperty] private ObservableCollection<Driver> _drivers = new();
    private readonly DriverManager _driverManager = new();
    private readonly AppDbContext _context = new ();
    
    [RelayCommand]
    private async Task GoBackToMainPage()
    {
        await Shell.Current.GoToAsync(nameof(MainPage), true);
    }

    [RelayCommand]
    private async Task DeleteDeviceAsync()
    {
        await ExecuteAsync(async () =>
        {
            bool userChoice = await Shell.Current.DisplayAlert("Delete Device", "Do you really want to delete this device?", "Yes", "No");
            if (Devices.Any() && userChoice)
            {
                foreach (var device in Devices)
                {
                    if (Device.device_id == device.device_id)
                    {
                        Devices.Remove(device);
                        await _context.DeleteItemAsync(Device);
                        await Shell.Current.GoToAsync(nameof(MainPage), true);
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
            var enumerable = devices as Device[] ?? devices.ToArray();
            if (enumerable.Any())
            {
                Devices.Clear();
                foreach (var device in enumerable)
                {
                   Devices.Add(device);
                }
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
        catch (Exception)
        {
            throw new Exception();
        }
        finally
        {
            //IsBusy = false;
            //BusyText = "Processing...";
        }
    }
    
    public async Task LoadDriversAsync()
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
                    if (Drivers.FirstOrDefault(p => p.driver_id == driver.driver_id) == null)
                    {
                        Drivers.Add(driver);
                    }
                }
            }
            
        });
    }

    [RelayCommand]
    private async Task EditDeviceAsync()
    {
        Driver currDeviceDriver = Driver;
        Device currDevice = Device;
        await Shell.Current.GoToAsync("EditDevicePage", true, new Dictionary<string, object>
        {
            { "Device", currDevice },
            { "Driver", currDeviceDriver }
        });
    }

    [RelayCommand]
    private async Task RunDeviceAsync()
    {
        await ExecuteAsync(async () =>
        {
            if (!_driverManager.IsDriverRunning(Driver.driver_name))
            {
                try
                {
                    _driverManager.StartDriverByName(Driver.driver_name);
                    Device.device_status = true;
                    var updDevice = Device.Clone();
                    var index = Devices.IndexOf(updDevice);
                    Devices.RemoveAt(index);
                    Devices.Insert(index, updDevice);
                    await Shell.Current.DisplayAlert("Device is running", "Device is started.", "Ok");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Attempt to start a working device", "This device is already launched.", "Ok");
            }
        });
    }

    [RelayCommand]
    private async Task StopDeviceAsync()
    {
        await ExecuteAsync(async() =>
        {
            if (_driverManager.IsDriverRunning(Driver.driver_name))
            {
                try
                {
                    _driverManager.StopDriverByName(Driver.driver_name);
                    Device.device_status = false;
                    var updDevice = Device.Clone();
                    var index = Devices.IndexOf(updDevice);
                    Devices.RemoveAt(index);
                    Devices.Insert(index, updDevice);
                 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                await Shell.Current.DisplayAlert("Device is stopped", "Device is stopped.", "Ok");
            }
            else
            {
                await Shell.Current.DisplayAlert("Attempt to stop a stopped device", "This device is already stopped.", "Ok");
            }
        });
    }
    
}