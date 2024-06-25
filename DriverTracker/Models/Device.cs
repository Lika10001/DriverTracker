using SQLite;

namespace DriverTracker.Models;

public class Device
{
    [PrimaryKey, AutoIncrement] public int device_id { get; set; }
    public string device_name { get; set; }
    public int device_driver_id { get; set; }
    public int device_status { get; set; }
    
    public bool IsDeviceDataNull()
    {
        return (device_name == null && device_driver_id == null && device_status == null);
    }
    
    public Device Clone() => MemberwiseClone() as Device;

}