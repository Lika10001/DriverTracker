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

    protected override void OnAppearing()
    {
        _=_viewModel.LoadDevicesAsync();
        _ = _viewModel.LoadDriversAsync();
    }
}