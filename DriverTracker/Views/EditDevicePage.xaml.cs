using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class EditDevicePage : ContentPage
{
    private EditDeviceViewModel _viewModel = new();
    public EditDevicePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }
    
    protected override void OnAppearing()
    {
        _viewModel.LoadDriversAsync();
    }
}