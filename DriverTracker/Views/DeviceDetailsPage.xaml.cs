using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class DeviceDetailsPage
{
    private DeviceDetailsViewModel _viewModel = new();
    public DeviceDetailsPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        await _viewModel.LoadDevicesAsync();
        await _viewModel.LoadDriversAsync();
    }
}