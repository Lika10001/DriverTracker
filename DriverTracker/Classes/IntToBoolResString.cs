using System.Globalization;

namespace DriverTracker.Classes;

public class IntToBoolResString : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType == typeof(int))
        {
            if (value is int intValue)
            {
                if (intValue == 0)
                {
                    return "false";
                }
                
                return "true";
            }
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType == typeof(string))
        {
            if (value is string stringValue)
            {
                if (stringValue.Equals("false"))
                {
                    return 0;
                }

                return 1;
            }
        }

        return value;
    }
}