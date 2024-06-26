using DriverTracker.ViewModels;
using DriverTracker.Views;
using Microsoft.Extensions.Logging;

namespace DriverTracker;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
    {
        builder
            .UseMauiApp<App>()
            //.UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
       
        

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<AppBDContext>();
        builder.Services.AddSingleton<SighInPage>();
        builder.Services.AddSingleton<SighUpPage>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<SighInViewModel>();
        builder.Services.AddSingleton<SighUpPageViewModel>();
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<AddDevicePage>();
        builder.Services.AddSingleton<DeviceDetailsPage>();
        builder.Services.AddSingleton<DeviceDetailsViewModel>();
        return builder;
    }
}