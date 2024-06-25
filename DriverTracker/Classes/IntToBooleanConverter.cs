using System.Globalization;

namespace DriverTracker.Classes;

public class IntToBooleanConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType == typeof(bool))
        {
            if (value is int intValue)
            {
                return (intValue == 0? false : true);
            }
        }
        else if (targetType == typeof(int))
        {
            if (value is bool boolValue)
            {
                return boolValue ? 1 : 0;
            }
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType == typeof(int))
        {
            if (value is bool boolValue)
            {
                return boolValue ? 1 : 0;
            }
        }
        else if (targetType == typeof(bool))
        {
            if (value is int intValue)
            {
                return intValue != 0;
            }
        }

        return value;
    }
}