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
  
    private readonly AppDbContext _context = new ();

    [RelayCommand]
    private async Task StopAllDrivers()
    {
        await ExecuteAsync(async () =>
        {
            Device currDevice = new();
            List<Device> devicesForUpdating = [..Devices];
            foreach (var device in devicesForUpdating)
            {
                if (device.device_status == 1)
                {
                    try
                    {
                        var currDriver = Drivers.FirstOrDefault(p => p.driver_id == device.device_driver_id);
                        if (currDriver != null) _driverManager.StopDriverByName(currDriver.driver_name);
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
                    if (Drivers.FirstOrDefault(p=>p.driver_id == driver.driver_id) == null)
                    {
                        Drivers.Add(driver);
                    }
                }
            }
        });
    }
    
    [RelayCommand]
    private async Task RunDriversForDevices()
    {
        await ExecuteAsync(async () =>
        {
            Device currDevice = new();
            List<Device> devicesForUpdating = [..Devices];
            foreach (var device in devicesForUpdating)
            {
                if (device.device_status == 0){
                    try
                    {
                        var driver =
                            Drivers.FirstOrDefault(predicate => predicate.driver_id == device.device_driver_id);
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
            var enumerable = devices as Device[] ?? devices.ToArray();
            if (enumerable.Any())
            {
                Devices.Clear();
                foreach (var device in enumerable)
                {
                    if (Devices.FirstOrDefault(p=>p.device_id == device.device_id) == null)
                    {
                        Devices.Add(device);
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