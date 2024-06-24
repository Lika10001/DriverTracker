using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Views;

namespace DriverTracker.ViewModels;

public partial class SighUpPageViewModel:ObservableObject
{
    [RelayCommand]
    private async Task NavigateToSighInAsync() {
        
        await Shell.Current.GoToAsync(nameof(SighInPage), true);
        
    }
}