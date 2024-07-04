using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DriverTracker.Models;

public partial class Device:ObservableObject
{
    [PrimaryKey, AutoIncrement] 
    public int device_id { get; set; }
    public string device_name { get; set; }
    public int device_driver_id { get; set; }

    public string device_info { get; set; }

    public bool device_status { get; set; }

    public bool IsDeviceDataNull()
    {
        return (device_name == null || device_driver_id == null || string.IsNullOrWhiteSpace(device_name));
    }

    public bool IsDeviceNameNull()
    {
        return (device_name == null || string.IsNullOrWhiteSpace(device_name));
    }
    
    public Device Clone() => MemberwiseClone() as Device;

}