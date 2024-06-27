

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
        Routing.RegisterRoute(nameof(DeviceDetailsPage), typeof(DeviceDetailsPage));
        Routing.RegisterRoute(nameof(AddDevicePage), typeof(AddDevicePage));
        Routing.RegisterRoute(nameof(EditDevicePage), typeof(EditDevicePage));
    }
}