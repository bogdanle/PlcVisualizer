using System;
using System.Windows.Data;

namespace UI.Controls.Converters
{
    public class TitleCaseConverter : IValueConverter
    {
        public static bool TitleCase { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = value as string;
            return TitleCase ? text : text?.ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
