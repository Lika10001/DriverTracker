using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Classes;
using DriverTracker.Models;
using DriverTracker.Views;

namespace DriverTracker.ViewModels;

public partial class SighInViewModel: ObservableObject
{
    [ObservableProperty]
    private string _userName;

    [ObservableProperty] 
    private string _userPassword;
    
    private readonly AppBDContext _context;
    
    private ObservableCollection<User> _users= new(); 
    
    public SighInViewModel(AppBDContext context)
    {
        _context = context;
    }
    
    public async Task LoadUsersAsync()
    {
        await ExecuteAsync(async () =>
        {
            var users = await _context.GetAllAsync<User>();
            if (users is not null && users.Any())
            {
                _users ??= new ObservableCollection<User>();
                foreach (var user in users)
                {
                    _users.Add(user);
                }
            }
        });
    }
    
    [RelayCommand]
    private async Task NavigateAsync(){
        if (Validator.IsLoginValid(UserName) && Validator.IsPasswordValid(UserPassword)) {
            
            if (_users.Any(p=>p.user_login == _userName && p.user_password == _userPassword))
            {
                await Shell.Current.GoToAsync(nameof(MainPage), true);
            }
            else
            {
                await Shell.Current.DisplayAlert("Sigh In Error", "This user is not registered. Check your login and password carefully.", "Try again"); 
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Sigh In Error", "Login or password is invalid. Check your login and password carefully.", "Try again"); 
        }
    }
    
    [RelayCommand]
    private async Task NavigateToSighUpAsync() {
        
        await Shell.Current.GoToAsync(nameof(SighUpPage), true);
        
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
                throw new Exception();
            }
            finally
            {
                //IsBusy = false;
                //BusyText = "Processing...";
            }
        }
}