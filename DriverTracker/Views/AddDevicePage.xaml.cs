using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class AddDevicePage
{
    private AddDeviceViewModel _viewModel = new();
    public AddDevicePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        _ = _viewModel.LoadDriversAsync();
        _viewModel.NewDevice = new();
        _viewModel.ChosenDriver = new();
    }
}