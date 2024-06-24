using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Models;
using DriverTracker.Views;

namespace DriverTracker.ViewModels;

public partial class SighInViewModel: ObservableObject
{
    [ObservableProperty]
    private string userName;

    [ObservableProperty] 
    private string userPassword;
    
    private List<User> _users= new List<User>(); 
    
    [RelayCommand]
    private async Task NavigateAsync(){
        if (Validator.CheckLogin(UserName) && Validator.CheckPassword(UserPassword))
        {
            // User user = await loginRepository.Login(UserName, UserPassword);
            await Shell.Current.GoToAsync(nameof(MainPage), true);
        }
    }
    
    [RelayCommand]
    private async Task NavigateToSighUpAsync() {
        
        await Shell.Current.GoToAsync(nameof(SighUpPage), true);
        
    }
}