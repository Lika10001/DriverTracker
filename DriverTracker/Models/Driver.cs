using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DriverTracker.Models;

public class Driver:ObservableObject
{
    [PrimaryKey, AutoIncrement] public int driver_id { get; set; }
    public string driver_name { get; set; }
    public string driver_ip { get; set; }
    public int driver_port { get; set; }

    public byte[] driver_file { get; set; }

    public Driver()
    {
        
    }

    public Driver(int driver_id, string driver_name, string driver_ip, int driver_port, byte[] driver_file)
    {
        this.driver_id = driver_id;
        this.driver_name = driver_name;
        this.driver_ip = driver_ip;
        this.driver_port = driver_port;
        this.driver_file = driver_file;
    }
    
    public bool IsDriverDataNull()
    {
        return (driver_name == null && driver_ip == null && driver_port == null);
    }
    
    public Driver Clone() => MemberwiseClone() as Driver;

}