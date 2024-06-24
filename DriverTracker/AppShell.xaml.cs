

using DriverTracker.Views;

namespace DriverTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(SighUpPage), typeof(SighUpPage));
        //Routing.RegisterRoute(nameof(SighInPage), typeof(SighInPage));
        Routing.RegisterRoute(nameof(DriverInfoPage), typeof(DriverInfoPage));
        Routing.RegisterRoute(nameof(AddDriverPage), typeof(AddDriverPage));
    }
}