using SQLite;

namespace DriverTracker.Models;

public class Driver
{
    [PrimaryKey, AutoIncrement] public int driver_id { get; set; }
    public string driver_name { get; set; }
    public string driver_ip { get; set; }
    public int driver_port { get; set; }
    
    public bool IsDriverDataNull()
    {
        return (driver_name == null && driver_ip == null && driver_port == null);
    }

}