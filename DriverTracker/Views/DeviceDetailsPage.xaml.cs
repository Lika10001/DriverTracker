using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverTracker.ViewModels;

namespace DriverTracker.Views;

public partial class DeviceDetailsPage : ContentPage
{
    public DeviceDetailsPage()
    {
        InitializeComponent();
        BindingContext = new DeviceDetailsViewModel();
    }
}