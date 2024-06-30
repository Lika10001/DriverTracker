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
    [ObservableProperty] private ObservableCollection<Device> _devices = new();
    [ObservableProperty] private ObservableCollection<Driver> _drivers = new();
    [ObservableProperty] private int _driverIndexForPicker;

    private readonly AppDbContext _context = new();
    
     private async Task GetDriverNamesFromCollection()
    {
        await ExecuteAsync(() =>
        {
            DriverNames.Clear();
            foreach (var driver in Drivers)
            {
                if (DriverNames.FirstOrDefault(p=>p == driver.driver_name) == null)
                {
                    DriverNames.Add(driver.driver_name);
                }
            }
            return Task.CompletedTask;
        });
    }
    
    private async Task LoadDevicesAsync()
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
                    if (Devices.FirstOrDefault(p=> p.device_id == device.device_id) == null)
                    {
                        Devices.Add(device);
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

            var currDriver = Drivers.FirstOrDefault(p => p.driver_id == Driver.driver_id);
            DriverIndexForPicker = Drivers.IndexOf(currDriver);
            await GetDriverNamesFromCollection();
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
    private async Task EditDeviceAndDriverAsync()
    {
        await LoadDevicesAsync();
         if (Device.IsDeviceNameNull())
        {
            await Shell.Current.DisplayAlert("Validation Error", "One of the fields is empty.", "Ok");
            return;
        }

        if (!(Validator.IsDeviceFieldValid(Device.device_name)))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Device name is too short.", "Ok");
            return;
        }
        
        if (Driver.driver_name =="")
        {
            await Shell.Current.DisplayAlert("Validation Error", "Driver name is empty.", "Ok");
            return;
        }
        
        if (!(Validator.IsIPValid(Driver.driver_ip) && Validator.IsPortValid(Driver.driver_port.ToString())))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Driver ip or port is incorrect.", "Ok");
            return;
        }

        if (Driver.driver_name == null)
        {
            Driver.driver_name = DriverNames[DriverIndexForPicker];
        }

        await ExecuteAsync(async () =>
        {
            try
            {
               if (Drivers.FirstOrDefault(p => p.driver_name == Driver.driver_name
                                                &&  p.driver_ip == Driver.driver_ip
                                                && p.driver_port == Driver.driver_port) == null)
               {
                   await _context.AddItemAsync(Driver);
                   Drivers.Add(Driver);
                   Device.device_driver_id = Drivers.Count;
                  
               }
               else
               {
                   var existDriver= Drivers.FirstOrDefault(p => p.driver_name == Driver.driver_name
                                                                && p.driver_ip == Driver.driver_ip
                                                                && p.driver_port == Driver.driver_port);
                   Device.device_driver_id = existDriver.driver_id;
               }
               await _context.UpdateItemAsync(Device);
               var oldDevice = Devices.FirstOrDefault(p=>p.device_id == Device.device_id);
               var index = Devices.IndexOf(oldDevice);
               Devices.RemoveAt(index);
               Devices.Insert(index, Device.Clone());

               var currDevice = Device;
               var currDriver = Driver;
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