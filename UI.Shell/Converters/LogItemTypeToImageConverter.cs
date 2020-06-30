using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UI.Shell.Converters
{
    public class LogItemTypeToImageConverter : IValueConverter
    {
        private static readonly ImageSource ErrorIcon = (ImageSource)Application.Current.FindResource("ErrorIcon");
        private static readonly ImageSource WarningIcon = (ImageSource)Application.Current.FindResource("WarningIcon");
        private static readonly ImageSource InfoIcon = (ImageSource)Application.Current.FindResource("InfoIcon");
        private static readonly ImageSource BugIcon = (ImageSource)Application.Current.FindResource("BugIcon");

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}