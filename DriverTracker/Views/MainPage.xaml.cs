using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class MainPage : ContentPage
{
    public MainPageViewModel _viewModel = new();
    public MainPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadDevicesAsync();
        _ = _viewModel.LoadDriversFromBD();
    }
}