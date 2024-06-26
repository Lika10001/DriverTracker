using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class AddDevicePage : ContentPage
{
    private AddDeviceViewModel _viewModel = new();
    public AddDevicePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        _viewModel.LoadDriversAsync();
    }
}