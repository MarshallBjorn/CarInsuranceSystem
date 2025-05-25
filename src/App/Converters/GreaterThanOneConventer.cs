using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace App.Converters
{
    public class GreaterThanOneConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is int intValue && intValue > 1;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
