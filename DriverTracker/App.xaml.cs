using DriverTracker.Models;

namespace DriverTracker;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
            //MainPage = new NavigationPage(new SighInPage());
            
    }
}