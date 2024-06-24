using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DriverTracker.ViewModels;

public partial class BaseViewModel: ObservableObject
{
    [ObservableProperty]
    public bool _isBusy;

    [ObservableProperty] 
    public string _title;

}