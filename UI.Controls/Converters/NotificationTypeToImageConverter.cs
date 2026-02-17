using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UI.Controls.Converters;

public class NotificationTypeToImageConverter : IValueConverter
{
    private static readonly ImageSource ErrorIcon;
    private static readonly ImageSource WarningIcon;
    private static readonly ImageSource InfoIcon;

    static NotificationTypeToImageConverter()
    {
        ErrorIcon = (ImageSource)Application.Current.Resources["StopIcon"];
        WarningIcon = (ImageSource)Application.Current.Resources["WarningIcon"];
        InfoIcon = (ImageSource)Application.Current.Resources["InfoIcon"];
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }

        switch ((NotificationMessageType)value)
        {
            case NotificationMessageType.Info:
                return InfoIcon;

            case NotificationMessageType.Error:
                return ErrorIcon;

            case NotificationMessageType.Warning:
                return WarningIcon;

            default:
                return null;
        }
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}