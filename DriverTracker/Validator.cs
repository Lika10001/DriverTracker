namespace DriverTracker;
//this class validates data according to its type.
//Methods return true, if data is correct, false - data is not correct.  
public static class Validator
{
    private const int PasswordLength = 1;
    private const int LoginLength = 1;
        
    public static bool CheckPassword(string data)
    {
        return ((data.Length > PasswordLength) && (!string.IsNullOrWhiteSpace(data)));
    }
    
    public static bool CheckLogin(string data)
    {
        return ((data.Length > LoginLength) && (!string.IsNullOrWhiteSpace(data)));
    }
}