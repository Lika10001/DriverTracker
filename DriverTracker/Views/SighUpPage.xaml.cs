using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class SighUpPage : ContentPage
{
    private SighUpPageViewModel _viewModel;
    public SighUpPage()
    {
        InitializeComponent();
        BindingContext = new SighUpPageViewModel();
       // _viewModel = viewModel;
    }
}