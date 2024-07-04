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
    private string _userName = String.Empty;

    [ObservableProperty] 
    private string _userPassword =String.Empty;
    
    private readonly AppDbContext _context = new();
    
    private ObservableCollection<User> _users= new(); 
    
    public async Task LoadUsersAsync()
    {
        await ExecuteAsync(async () =>
        {
            var users = await _context.GetAllAsync<User>();
            var enumerable = users as User[] ?? users.ToArray();
            if (enumerable.Any())
            {
                _users.Clear();
                foreach (var user in enumerable)
                {
                    _users.Add(user);
                }
            }
        });
    }
    
    [RelayCommand]
    private async Task NavigateAsync(){
        if (Validator.IsLoginValid(UserName) && Validator.IsPasswordValid(UserPassword)) {
            
            if (_users.Any(p=>p.user_login == UserName && p.user_password == UserPassword))
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