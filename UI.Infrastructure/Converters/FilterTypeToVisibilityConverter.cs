using System;
using System.Windows;
using System.Windows.Data;
using UI.Infrastructure.Models;

namespace UI.Infrastructure.Converters
{
    /// <summary>
    /// Converter used to hide/show the image control in filter button depending on the filter type.
    /// If FilterType is set to none, no image will be displayed.
    /// </summary>        
    public class FilterTypeToVisibilityConverter : IValueConverter
    {        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var obj = (FilterDefinition.FilterType)value;
            return (obj == FilterDefinition.FilterType.None || obj == FilterDefinition.FilterType.Custom) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}