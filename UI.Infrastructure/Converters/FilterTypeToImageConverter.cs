using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UI.Infrastructure.Models;

namespace UI.Infrastructure.Converters
{
    /// <summary>
    /// Converter used to display icon in filter button depending on the filter type.    
    /// </summary>        
    public class FilterTypeToImageConverter : IValueConverter
    {
        private static readonly ImageSource _errorIcon = (ImageSource)Application.Current.Resources["ErrorIcon"];
        private static readonly ImageSource _warningIcon = (ImageSource)Application.Current.Resources["WarningIcon"];
        private static readonly ImageSource _editIcon = (ImageSource)Application.Current.Resources["PencilIcon"];

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((FilterDefinition.FilterType)value)
            {
                case FilterDefinition.FilterType.Override:
                    return _editIcon;

                case FilterDefinition.FilterType.Warning:
                    return _warningIcon;

                case FilterDefinition.FilterType.Error:
                    return _errorIcon;

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}