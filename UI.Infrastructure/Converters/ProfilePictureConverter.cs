using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UI.Infrastructure.Converters
{
    public class ProfilePictureConverter : IValueConverter
    {
        private static readonly ImageSource _userIcon = (ImageSource)Application.Current.Resources["UserIcon"];

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] picture = value as byte[];
            return picture?.Length > 0 ? value : _userIcon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}