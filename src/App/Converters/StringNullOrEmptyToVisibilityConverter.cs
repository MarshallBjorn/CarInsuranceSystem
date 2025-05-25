namespace App.Converters;

using Avalonia.Data.Converters;
using System;
using System.Globalization;

public class StringNullOrEmptyToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Returns true if value is not null or empty => visible
        return !string.IsNullOrEmpty(value as string);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
