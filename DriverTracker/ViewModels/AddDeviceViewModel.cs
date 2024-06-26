using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Classes;
using DriverTracker.Models;
using DriverTracker.Views;
using Device = DriverTracker.Models.Device;

namespace DriverTracker.ViewModels;

[QueryProperty(nameof(ObservableCollection<Driver>), "Drivers")]
[QueryProperty(nameof(List<string>), "DriverNames")]

public partial class AddDeviceViewModel : ObservableObject
{
    [ObservableProperty] private Device _newDevice= new ();
    [ObservableProperty] private Driver _choosenDriver = new();
    
    [ObservableProperty] private ObservableCollection<Driver> _drivers;
    [ObservableProperty] private List<string> _driverNames;
    
    [ObservableProperty] private ObservableCollection<Device> _devices;
    
    private readonly AppBDContext _context = new ();
    [RelayCommand]
    public async Task AddDeviceAndDriver()
    {
        
    }
    
    [RelayCommand]
    private async Task SaveUserAsync()
    {
        await LoadDevicesAsync();

        if (_newDevice.IsDeviceDataNull())
        {
            await Shell.Current.DisplayAlert("Validation Error", "One of the fields is empty.", "Ok");
            return;
        }

        if (!(Validator.IsDeviceFieldValid(_newDevice.device_name) && Validator.IsDeviceFieldValid(_newDevice.device_status.ToString())))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Password or Login is too short.", "Ok");
            return;
        }
        
        if (!(Validator.IsIPValid(_choosenDriver.driver_ip) && Validator.IsPortValid(_choosenDriver.driver_port.ToString())))
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
            // Create user
            try
            {
                var driverFromDB = await _context.GetItemByKeyAsync<Driver>(_choosenDriver.driver_id);
                if (driverFromDB == null)
                {
                    await _context.AddItemAsync<Driver>(_choosenDriver);
                }
                await _context.AddItemAsync<Device>(_newDevice);
                _devices.Add(_newDevice);
                await Shell.Current.GoToAsync(nameof(MainPage), true);
                await Shell.Current.DisplayAlert("Success", "Your device has been successfully created.",
                    "Ok");
                await Shell.Current.GoToAsync(nameof(MainPage), true);
            }
            catch (Exception)
            {
                throw new Exception();
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