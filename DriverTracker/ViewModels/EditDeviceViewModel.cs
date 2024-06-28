using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Classes;
using DriverTracker.Models;
using DriverTracker.Views;
using Device = DriverTracker.Models.Device;

namespace DriverTracker.ViewModels;

[QueryProperty(nameof(Device), "Device")]
[QueryProperty(nameof(Driver), "Driver")]
public partial class EditDeviceViewModel : ObservableObject
{
    [ObservableProperty] private Device _device = new();
    [ObservableProperty] private Driver _driver = new();
    [ObservableProperty] private ObservableCollection<string> _driverNames = new();
    [ObservableProperty] private ObservableCollection<Device> _devices;
    [ObservableProperty] private ObservableCollection<Driver> _drivers;
    [ObservableProperty] private int _driverIndexForPicker = 0;

    private AppDbContext _context = new();
    
     public async Task GetDriverNamesFromCollection()
    {
        await ExecuteAsync(async () =>
        {
            _driverNames.Clear();
            foreach (var driver in _drivers)
            {
                if (_driverNames.FirstOrDefault(p=>p == driver.driver_name) == null)
                {
                    _driverNames.Add(driver.driver_name);
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
                    if (_devices.FirstOrDefault(p=> p.device_id == device.device_id) == null)
                    {
                        _devices.Add(device);
                    }
                }
            }
        });
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

            var currDriver = _drivers.FirstOrDefault(p => p.driver_id == _driver.driver_id);
            _driverIndexForPicker = _drivers.IndexOf(currDriver);
            await GetDriverNamesFromCollection();
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
    private async Task EditDeviceAndDriverAsync()
    {
        await LoadDevicesAsync();
         if (_device.IsDeviceNameNull())
        {
            await Shell.Current.DisplayAlert("Validation Error", "One of the fields is empty.", "Ok");
            return;
        }

        if (!(Validator.IsDeviceFieldValid(_device.device_name)))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Device name is too short.", "Ok");
            return;
        }
        
        if (_driver.driver_name == null)
        {
            await Shell.Current.DisplayAlert("Validation Error", "Driver name is empty.", "Ok");
            return;
        }
        
        if (!(Validator.IsIPValid(_driver.driver_ip) && Validator.IsPortValid(_driver.driver_port.ToString())))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Driver ip or port is incorrect.", "Ok");
            return;
        }

        await ExecuteAsync(async () =>
        {
            try
            {
               // сначала нужно проверить драйвер, новый это или существующий
               if (_drivers.FirstOrDefault(p => p.driver_name == _driver.driver_name
                                                &&  p.driver_ip == _driver.driver_ip
                                                && p.driver_port == _driver.driver_port) == null)
               {
                   await _context.AddItemAsync<Driver>(_driver);
                   _drivers.Add(_driver);
                   _device.device_driver_id = _drivers.Count;
                  
               }
               else
               {
                   var existDriver= _drivers.FirstOrDefault(p => p.driver_name == _driver.driver_name
                                                                && p.driver_ip == _driver.driver_ip
                                                                && p.driver_port == _driver.driver_port);
                   _device.device_driver_id = existDriver.driver_id;
               }
               await _context.UpdateItemAsync<Device>(_device);
               var oldDevice = _devices.FirstOrDefault(p=>p.device_id == _device.device_id);
               var index = _devices.IndexOf(oldDevice);
               _devices.RemoveAt(index);
               _devices.Insert(index, _device.Clone());

               var currDevice = _device;
               var currDriver = _driver;
                await Shell.Current.GoToAsync(nameof(DeviceDetailsPage), true, new Dictionary<string, object>()
                {
                    {"Device", currDevice},
                    {"Driver", currDriver}
                });
                await Shell.Current.DisplayAlert("Success", "Your device has been successfully edited.",
                    "Ok");
            }
            catch (Exception)
            {
                throw new Exception();
            }
        });

    }
    
}