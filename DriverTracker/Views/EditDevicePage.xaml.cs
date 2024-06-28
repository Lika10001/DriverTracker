using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class EditDevicePage
{
    private EditDeviceViewModel _viewModel = new();
    public EditDevicePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }
    
    protected override void OnAppearing()
    {
        _ = _viewModel.LoadDriversAsync();
    }
}