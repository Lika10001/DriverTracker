using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class SighInPage
{
    private SighInViewModel _viewModel = new();
    
    public SighInPage ()
    {
        InitializeComponent();
        BindingContext =  _viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadUsersAsync();
        _viewModel.UserName = "";
        _viewModel.UserPassword = "";
    }
}