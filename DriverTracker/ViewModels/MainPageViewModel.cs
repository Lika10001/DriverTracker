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
    
    private DriverManager _driverManager = new();
  
    private readonly AppDbContext _context = new ();

    [RelayCommand]
    public async Task StopAllDrivers()
    {
        await ExecuteAsync(async () =>
        {
            Device currDevice = new();
            List<Device> devicesForUpdating = new List<Device>(_devices);
            foreach (var device in devicesForUpdating)
            {
                if (device.device_status == 1)
                {
                    try
                    {
                        _driverManager.StopDriverByName(
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
        });
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
                _drivers.Clear();
                foreach (var driver in drivers)
                {
                    if (_drivers.FirstOrDefault(p=>p.driver_id == driver.driver_id) == null)
                    {
                        _drivers.Add(driver);
                    }
                }
            }
        });
    }
    
    [RelayCommand]
    public async Task RunDriversForDevices()
    {
        await ExecuteAsync(async () =>
        {
            Device currDevice = new();
            List<Device> devicesForUpdating = new List<Device>(_devices);
            foreach (var device in devicesForUpdating)
            {
                if (device.device_status == 0){
                    try
                    {
                        var driver =
                            _drivers.FirstOrDefault(predicate => predicate.driver_id == device.device_driver_id);
                        if (driver != null)
                        {
                            _driverManager.StartDriverByName(driver.driver_name);
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
                _devices.Clear();
                foreach (var device in devices)
                {
                    if (_devices.FirstOrDefault(p=>p.device_id == device.device_id) == null)
                    {
                        _devices.Add(device);
                    }
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
                    var oldDevice = _devices.FirstOrDefault(p=>p.device_id == device.device_id);
                    var index = _devices.IndexOf(oldDevice);
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
    private async Task NavigateToAddDeviceAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddDevicePage), true);
    }
    
    [RelayCommand]
    private async Task NavigateToSighInPageAsync()
    {
        await Shell.Current.GoToAsync(nameof(SighInPage), true);
    }

    
}