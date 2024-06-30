using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriverTracker.Classes;
using DriverTracker.Models;
using DriverTracker.Views;

namespace DriverTracker.ViewModels;

public partial class SighUpPageViewModel:ObservableObject
{
    [ObservableProperty] private string? _userConfirmedPassword;

    [ObservableProperty] private User _newUser = new();
    
    private ObservableCollection<User> _users= new(); 
    
    private readonly AppDbContext _context = new ();
    
    
    [RelayCommand]
    private async Task NavigateToSighInAsync() {

        try
        {
            await Shell.Current.GoToAsync("///SighInPage", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    [RelayCommand]
    private async Task SaveUserAsync()
    {
        await LoadUsersAsync();

        if (NewUser.IsUserDataNull())
        {
            await Shell.Current.DisplayAlert("Validation Error", "One of the fields is empty.", "Ok");
            return;
        }

        if (!(Validator.IsLoginValid(NewUser.user_login) && Validator.IsPasswordValid(NewUser.user_password)))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Password or Login is too short.", "Ok");
            return;
        }

        if (NewUser.user_password != UserConfirmedPassword)
        {
            await Shell.Current.DisplayAlert("Validation Error", "Your password is not equal to confirmed password.",
                "Ok");
            return;
        }

        if (_users.Any(p => p.user_login == NewUser.user_login))
        {
            await Shell.Current.DisplayAlert("Validation Error", "This login already exists. Choose another one.",
                "Ok");
            return;
        }

        await ExecuteAsync(async () =>
        {
            try
            {
                await _context.AddItemAsync(NewUser);
                _users.Add(NewUser);
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