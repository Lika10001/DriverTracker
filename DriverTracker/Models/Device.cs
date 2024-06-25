using SQLite;

namespace DriverTracker.Models;

public class Device
{
    [PrimaryKey, AutoIncrement] public int device_id { get; set; }
    public string device_name { get; set; }
    public string device_driver_name { get; set; }
    public string device_status { get; set; }
    
    public bool IsDeviceDataNull()
    {
        return (device_name == null && device_driver_name == null && device_status == null);
    }

}