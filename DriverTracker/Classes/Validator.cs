namespace DriverTracker.Classes;
//this class validates data according to its type.
//Methods return true, if data is correct, false - data is not correct.  
public static class Validator
{
    private const int PasswordLength = 3;
    private const int LoginLength = 3;
    private const int MaxPortValue = 65535;
    private const int MinPortValue = 0;
        
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
        try
        {
            int value = Convert.ToInt32(data);
        }
        catch (Exception)
        {
            return false;
        }

        return ((Convert.ToInt32(data) <= MaxPortValue) &&(Convert.ToInt32(data) >= MinPortValue));
    }
    
    public static bool IsIPValid(string data)
    {
        if (!data.Contains('.') && !data.Contains(':'))
            return false;

        return (data.Contains(':') != true && IsValidIPv4Address(data));
    }

    private static bool IsValidIPv4Address(string ipAddress)
    {
        string[] octets = ipAddress.Split('.');
        if (octets.Length != 4)
            return false;
        foreach (string octet in octets)
        {
            if (!int.TryParse(octet, out int value))
                return false;
            if (value < 0 || value > 255)
                return false;
        }
        return true;
    }
        
    public static bool IsDeviceFieldValid(string data)
    {
        return ((data.Length > PasswordLength) && (!string.IsNullOrWhiteSpace(data)));
    }
    
    
}