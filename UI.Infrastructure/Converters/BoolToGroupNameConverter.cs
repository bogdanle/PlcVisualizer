using System;
using System.Globalization;
using System.Windows.Data;

namespace UI.Infrastructure.Converters
{
    public class BoolToGroupNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool)value;
            return !flag ? string.Empty : (parameter ?? "FilterGroup");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
