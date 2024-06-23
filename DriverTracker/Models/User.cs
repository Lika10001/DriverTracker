using SQLite;

namespace DriverTracker.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int UserId { get; set; }
    public String UserName { get; set; }
    public String UserLogin { get; set; }
    public String UserPassword { get; set; }
}