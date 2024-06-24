using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class SighInPage : ContentPage
{
    private readonly SighInViewModel _viewModel;
    
    public SighInPage (SighInViewModel viewModel)
    {
        InitializeComponent();
        BindingContext =  viewModel;
        _viewModel = viewModel;
    }
}