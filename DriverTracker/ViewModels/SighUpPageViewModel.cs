using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Classes;
using DriverTracker.Models;
using DriverTracker.Views;

namespace DriverTracker.ViewModels;

public partial class SighUpPageViewModel:ObservableObject
{
    [ObservableProperty] private string _userConfirmedPassword;

    [ObservableProperty] private User _newUser = new();
    
    private ObservableCollection<User> _users= new(); 
    
    private readonly AppBDContext _context = new ();
    
    
    [RelayCommand]
    private async Task NavigateToSighInAsync() {
        
        await Shell.Current.GoToAsync(nameof(SighInPage), true);
        
    }

    [RelayCommand]
    private async Task SaveUserAsync()
    {
        await LoadUsersAsync();

        if (_newUser.IsUserDataNull())
        {
            await Shell.Current.DisplayAlert("Validation Error", "One of the fields is empty.", "Ok");
            return;
        }

        if (!(Validator.IsLoginValid(_newUser.user_login) && Validator.IsPasswordValid(_newUser.user_password)))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Password or Login is too short.", "Ok");
            return;
        }

        if (_newUser.user_password != _userConfirmedPassword)
        {
            await Shell.Current.DisplayAlert("Validation Error", "Your password is not equal to confirmed password.",
                "Ok");
            return;
        }

        if (_users.Any(p => p.user_login == _newUser.user_login))
        {
            await Shell.Current.DisplayAlert("Validation Error", "This login already exists. Choose another one.",
                "Ok");
            return;
        }

        await ExecuteAsync(async () =>
        {
            // Create user
            try
            {
                await _context.AddItemAsync<User>(_newUser);
                _users.Add(_newUser);
                await Shell.Current.GoToAsync(nameof(MainPage), true);
                await Shell.Current.DisplayAlert("Successful sigh up", "Your account has been successfully created.",
                    "Ok");
                
            }
            catch (Exception)
            {
                throw new Exception();
            }
        });
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