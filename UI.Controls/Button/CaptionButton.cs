using System;
using System.Windows;

namespace UI.Controls;

[Flags]
public enum CaptionButtonType
{
    None = 0x00,
    Minimize = 0x01,
    Restore = 0x02,
    Maximize = 0x04,
    Close = 0x08
}

public class CaptionButton : System.Windows.Controls.Button
{
    public static readonly DependencyProperty ButtonTypeProperty =
        DependencyProperty.Register(nameof(ButtonType), typeof(CaptionButtonType), typeof(CaptionButton), new PropertyMetadata(CaptionButtonType.None, OnButtonTypePropertyChanged));

    public CaptionButton()
    {
        Style = (Style)Application.Current.Resources["CaptionButtonStyle"];
    }

    public CaptionButtonType ButtonType
    {
        get => (CaptionButtonType)GetValue(ButtonTypeProperty);
        set => SetValue(ButtonTypeProperty, value);
    }

    private static void OnButtonTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var obj = (CaptionButton)d;
        if (obj != null && e.NewValue != null)
        {
            switch ((CaptionButtonType)e.NewValue)
            {
                case CaptionButtonType.Close:
                    obj.Style = (Style)Application.Current.Resources["CloseCaptionButtonStyle"];
                    break;

                case CaptionButtonType.Maximize:
                    obj.Style = (Style)Application.Current.Resources["MaximizeCaptionButtonStyle"];
                    break;

                case CaptionButtonType.Minimize:
                    obj.Style = (Style)Application.Current.Resources["MinimizeCaptionButtonStyle"];
                    break;

                case CaptionButtonType.Restore:
                    obj.Style = (Style)Application.Current.Resources["RestoreCaptionButtonStyle"];
                    break;
            }
        }
    }
}