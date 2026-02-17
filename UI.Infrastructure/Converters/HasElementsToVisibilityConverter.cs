using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UI.Infrastructure.Converters;

public class HasElementsToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string param = (string)parameter;

        if (value is ICollection col)
        {
            return col.Count > 0 ? Visibility.Visible : param == "Hidden" ? Visibility.Hidden : Visibility.Collapsed;
        }

        return param == "Hidden" ? Visibility.Hidden : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }        
}