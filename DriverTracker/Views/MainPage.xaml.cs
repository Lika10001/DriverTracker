using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class MainPage : ContentPage
{
    private MainPageViewModel _viewModel = new();
    public MainPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadDevicesAsync();
        _viewModel.LoadDriversFromBD();
    }
}