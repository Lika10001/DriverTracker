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
    [ObservableProperty] private Device _device;
    [ObservableProperty] private Driver _driver;
    
    [RelayCommand]
    public async Task GoBackToMainPage()
    {
        await Shell.Current.GoToAsync(nameof(MainPage), true);
    }

    
}