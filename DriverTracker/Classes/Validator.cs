namespace DriverTracker.Classes;
//this class validates data according to its type.
//Methods return true, if data is correct, false - data is not correct.  
public static class Validator
{
    private const int PasswordLength = 1;
    private const int LoginLength = 1;
        
    public static bool IsPasswordValid(string data)
    {
        return ((data.Length > PasswordLength) && (!string.IsNullOrWhiteSpace(data)));
    }
    
    public static bool IsLoginValid(string data)
    {
        return ((data.Length > LoginLength) && (!string.IsNullOrWhiteSpace(data)));
    }
    
    public static bool IsPortValid(string data)
    {
        return ((data.Length > PasswordLength) && (!string.IsNullOrWhiteSpace(data)));
    }
    
    public static bool IsIPValid(string data)
    {
        return ((data.Length > LoginLength) && (!string.IsNullOrWhiteSpace(data)));
    }
    
    public static bool IsDeviceFieldValid(string data)
    {
        return ((data.Length > PasswordLength) && (!string.IsNullOrWhiteSpace(data)));
    }
    
    
}