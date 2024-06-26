using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Views;

namespace DriverTracker.ViewModels;

public partial class DeviceDetailsViewModel:ObservableObject
{
    [RelayCommand]
    public async Task GoBackToMainPage()
    {
        await Shell.Current.GoToAsync(nameof(MainPage), true);
    }
}