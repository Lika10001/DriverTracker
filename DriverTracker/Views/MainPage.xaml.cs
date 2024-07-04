using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel = new();
    public MainPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }
    
    protected async override void OnAppearing()
    {
        base.OnAppearing(); 
        await _viewModel.LoadDevicesAsync();
        await _viewModel.LoadDriversFromBd();
        _viewModel.CheckAllDriversForRunning();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        //_ = _viewModel.StopAllDriversCommand;
    }
}