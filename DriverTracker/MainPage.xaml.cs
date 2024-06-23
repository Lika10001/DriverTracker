using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverTracker;

public partial class MainPage : ContentPage
{
    public AppBDContext database;
    public MainPage()
    {
        InitializeComponent();
        database = new AppBDContext();
        var users = database.GetAllUsersAsync();
        
    }
}