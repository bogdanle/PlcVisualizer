using System;
using System.Windows.Data;

namespace UI.Controls.Converters;

public class ToLowercaseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        string text = value as string;
        return text?.ToLower();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}