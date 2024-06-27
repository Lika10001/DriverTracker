using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Classes;
using DriverTracker.Models;
using DriverTracker.Views;
using Device = DriverTracker.Models.Device;

namespace DriverTracker.ViewModels;


public partial class AddDeviceViewModel : ObservableObject
{
    [ObservableProperty] private Device _newDevice= new ();
    [ObservableProperty] private Driver _chosenDriver = new();
    
    [ObservableProperty] private ObservableCollection<Driver> _drivers;
    [ObservableProperty] private string _selectedDriverNameFromPicker;
    [ObservableProperty] private ObservableCollection<string> _driverNames = new();
    
    [ObservableProperty] private ObservableCollection<Device> _devices; 
    private readonly AppBDContext _context = new ();

    
    [RelayCommand]
    private async Task AddDeviceAndDriver()
    {
        await LoadDevicesAsync();
        
        if (_newDevice.IsDeviceNameNull())
        {
            await Shell.Current.DisplayAlert("Validation Error", "One of the fields is empty.", "Ok");
            return;
        }

        if (!(Validator.IsDeviceFieldValid(_newDevice.device_name)))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Device name is too short.", "Ok");
            return;
        }
        
        if (_chosenDriver.driver_name == null)
        {
            await Shell.Current.DisplayAlert("Validation Error", "Driver name is empty.", "Ok");
            return;
        }
        
        if (!(Validator.IsIPValid(_chosenDriver.driver_ip) && Validator.IsPortValid(_chosenDriver.driver_port.ToString())))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Driver ip or port is incorrect.", "Ok");
            return;
        }

        if (_devices.Any(p => p.device_name == _newDevice.device_name))
        {
            await Shell.Current.DisplayAlert("Validation Error", "This device already exists. Choose another name.",
                "Ok");
            return;
        }

        await ExecuteAsync(async () =>
        {
            try
            {
               // _chosenDriver.driver_name = _selectedDriverNameFromPicker;
               if (_drivers.FirstOrDefault(p => p.driver_name == _chosenDriver.driver_name
                                                &&  p.driver_ip == _chosenDriver.driver_ip
                                                && p.driver_port == _chosenDriver.driver_port) == null)
               {
                   await _context.AddItemAsync<Driver>(_chosenDriver);
                   _drivers.Add(_chosenDriver);
                   _newDevice.device_driver_id = _drivers.Count;
                  
               }
               else
               {
                   var existDriver= _drivers.FirstOrDefault(p => p.driver_name == _chosenDriver.driver_name
                                                                && p.driver_ip == _chosenDriver.driver_ip
                                                                && p.driver_port == _chosenDriver.driver_port);
                   _newDevice.device_driver_id = existDriver.driver_id;
               }
               await _context.AddItemAsync<Device>(_newDevice);
                _devices.Add(_newDevice);
                await Shell.Current.GoToAsync(nameof(MainPage), true);
                await Shell.Current.DisplayAlert("Success", "Your device has been successfully created.",
                    "Ok");
            }
            catch (Exception)
            {
                throw new Exception();
            }
        });
    }

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
    
}