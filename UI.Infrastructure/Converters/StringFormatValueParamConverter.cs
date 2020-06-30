using System;
using System.Windows.Data;

namespace UI.Infrastructure.Converters
{
    public class StringFormatValueParamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is null || value is null)
            {
                return value; 
            }
            else 
            {
                return $"{value} {parameter}";
            }           
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
