using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class SighUpPage
{
    private SighUpPageViewModel _viewModel = new();
    public SighUpPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
       //BindingContext = viewModel;
        //_viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        await _viewModel.LoadUsersAsync();
        _viewModel.NewUser = new();
    }
}