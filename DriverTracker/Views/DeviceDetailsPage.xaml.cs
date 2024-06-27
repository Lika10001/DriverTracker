using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class DeviceDetailsPage : ContentPage
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