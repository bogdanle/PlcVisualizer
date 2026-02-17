using System;
using System.Globalization;
using System.Windows.Data;

namespace UI.Infrastructure.Converters;

public class CountToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int count = (int)value;
        string text = (string)parameter;
        if (count != 1)
        {
            text += 's';
        }

        return $"{count} {text}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}