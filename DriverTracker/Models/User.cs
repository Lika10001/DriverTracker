using SQLite;

namespace DriverTracker.Models;

public class User
{
    [PrimaryKey, AutoIncrement] 
    public int user_id { get; set; }
    public string user_login { get; set; }
    public string user_password { get; set; }
    public string user_name { get; set; }
    
    public bool IsUserDataNull()
    {
        return ((user_login == null) && (user_password == null) && (user_name == null));
    }

}