using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UI.Shell.Converters
{
    public class MenuLabelToImageConverter : IValueConverter
    {
        private static readonly ImageSource OutputIcon = (ImageSource)Application.Current.Resources["ReportWhiteIcon"];
        private static readonly ImageSource AdminIcon = (ImageSource)Application.Current.Resources["SettingsIcon"];
        private static readonly ImageSource DefaultIcon = (ImageSource)Application.Current.Resources["QuestionIconWhite"];

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((string)value)
            {
                case "Data Output":
                    return OutputIcon;

                case "Admin":
                    return AdminIcon;                
            }

            return DefaultIcon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
