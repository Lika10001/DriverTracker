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
    
    [ObservableProperty] private ObservableCollection<Driver> _drivers = new();
    [ObservableProperty] private string _selectedDriverNameFromPicker = "";
    [ObservableProperty] private ObservableCollection<string> _driverNames = new();
    
    [ObservableProperty] private ObservableCollection<Device> _devices = new(); 
    private readonly AppDbContext _context = new ();

    
    [RelayCommand]
    private async Task AddDeviceAndDriver()
    {
        await LoadDevicesAsync();
        
        if (NewDevice.IsDeviceNameNull())
        {
            await Shell.Current.DisplayAlert("Validation Error", "One of the fields is empty.", "Ok");
            return;
        }

        if (!(Validator.IsDeviceFieldValid(NewDevice.device_name)))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Device name is too short.", "Ok");
            return;
        }
        
        if (ChosenDriver.driver_name == "")
        {
            await Shell.Current.DisplayAlert("Validation Error", "Driver name is empty.", "Ok");
            return;
        }
        
        if (!(Validator.IsIPValid(ChosenDriver.driver_ip) && Validator.IsPortValid(ChosenDriver.driver_port.ToString())))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Driver ip or port is incorrect.", "Ok");
            return;
        }

        if (Devices.Any(p => p.device_name == NewDevice.device_name))
        {
            await Shell.Current.DisplayAlert("Validation Error", "This device already exists. Choose another name.",
                "Ok");
            return;
        }

        await ExecuteAsync(async () =>
        {
            try
            {
                var currDriver = Drivers.FirstOrDefault(p => p.driver_name == ChosenDriver.driver_name);
               if (currDriver.driver_ip != ChosenDriver.driver_ip || currDriver.driver_port != ChosenDriver.driver_port)
               {
                   ChosenDriver.driver_id = currDriver.driver_id;
                   await _context.UpdateItemAsync(ChosenDriver);
                   var oldDriver = Drivers.FirstOrDefault(p=>p.driver_id == ChosenDriver.driver_id);
                   if (oldDriver != null)
                   {
                       int index = Drivers.IndexOf(oldDriver);
                       Drivers.RemoveAt(index);
                       Drivers.Insert(index, ChosenDriver.Clone());
                       NewDevice.device_driver_id = Drivers.Count;
                   }

               }
               else
               {
                   var existDriver= Drivers.FirstOrDefault(p => p.driver_name == ChosenDriver.driver_name
                                                                && p.driver_ip == ChosenDriver.driver_ip
                                                                && p.driver_port == ChosenDriver.driver_port);
                   if (existDriver != null) NewDevice.device_driver_id = existDriver.driver_id;
               }
               await _context.AddItemAsync(NewDevice);
                Devices.Add(NewDevice);
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

    private async Task GetDriverNamesFromCollection()
    {
        await ExecuteAsync( () =>
        {
            DriverNames.Clear();
            foreach (var driver in Drivers)
            {
                if (DriverNames.FirstOrDefault(p => p == driver.driver_name) == null)
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
        catch(Exception ex)
        {
            throw new Exception();
        }
        finally
        {
            //IsBusy = false;
            //BusyText = "Processing...";
        }
    }
    
}