using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty] private ObservableCollection<Device> _devices;
    [ObservableProperty] private ObservableCollection<Driver> _drivers;
    private readonly AppDbContext _context = new ();
    
    [RelayCommand]
    public async Task GoBackToMainPage()
    {
        await Shell.Current.GoToAsync(nameof(MainPage), true);
    }

    [RelayCommand]
    public async Task DeleteDeviceAsync()
    {
        await ExecuteAsync(async () =>
        {
            bool userChoice = await Shell.Current.DisplayAlert("Delete Device", "Do you really want to delete this device?", "Yes", "No");
            if (_devices.Any() && userChoice)
            {
                foreach (var device in _devices)
                {
                    if (_device.device_id == device.device_id)
                    {
                        _devices.Remove(device);
                        await _context.DeleteItemAsync(_device);
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
            if (devices is not null && devices.Any())
            {
                _devices ??= new ObservableCollection<Device>();
                _devices.Clear();
                foreach (var device in devices)
                {
                   _devices.Add(device);
                }
            }

            Device = _devices.FirstOrDefault(p => p.device_id == _device.device_id);
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
    
    public async Task LoadDriversAsync()
    {
        await ExecuteAsync(async () =>
        {
            var drivers = await _context.GetAllAsync<Driver>();
            if (drivers is not null && drivers.Any())
            {
                _drivers ??= new ObservableCollection<Driver>();
                _drivers.Clear();
                foreach (var driver in drivers)
                {
                    if (_drivers.FirstOrDefault(p => p.driver_id == driver.driver_id) == null)
                    {
                        _drivers.Add(driver);
                    }
                }
            }

            Driver = _drivers.FirstOrDefault(p => p.driver_id == _driver.driver_id);
        });
    }

    [RelayCommand]
    public async Task EditDeviceAsync()
    {
        Driver currDeviceDriver = new Driver();
        currDeviceDriver = _driver;
        Device currDevice = new Device();
        currDevice = _device;
        if (currDevice == null || currDeviceDriver == null)
        {
            await Shell.Current.DisplayAlert("Details error", "No details found about this device", "Ok");
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(EditDevicePage), true, new Dictionary<string, object>
            {
                { "Device", currDevice },
                { "Driver", currDeviceDriver }
            });
        }
    }
    
}