using SQLite;

namespace DriverTracker.Models;

public class User
{
    [PrimaryKey, AutoIncrement] public int UserId { get; set; }
    public string UserName { get; set; }
    public string UserLogin { get; set; }
    public string UserPassword { get; set; }
}