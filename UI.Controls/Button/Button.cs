using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UI.Controls
{
    public enum ButtonStyle
    {
        Standard,
        Rounded,
        Toolbar
    }

    public class Button : System.Windows.Controls.Button
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(Button), new PropertyMetadata(null, OnImageSourcePropertyChanged));
        public static readonly DependencyProperty DropDownMenuProperty = DependencyProperty.Register("DropDownMenu", typeof(ContextMenu), typeof(Button), new PropertyMetadata(null));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(BitmapImage), typeof(Button), new PropertyMetadata(null, OnImagePropertyChanged));
        public static readonly DependencyProperty IsBorderlessProperty = DependencyProperty.Register("IsBorderless", typeof(bool), typeof(Button), new PropertyMetadata(false, OnBorderlessChanged));
        public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(Button), new PropertyMetadata(new Thickness(5)));
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(ButtonStyle), typeof(Button), new PropertyMetadata(ButtonStyle.Standard, OnButtonStylePropertyChanged));
        public static readonly DependencyProperty IsRecommendedProperty = DependencyProperty.Register("IsRecommended", typeof(bool), typeof(Button), new PropertyMetadata(false, OnIsRecommendedPropertyChanged));
        public static readonly DependencyProperty RecommendedActionProperty = DependencyProperty.Register("RecommendedAction", typeof(string), typeof(Button), new PropertyMetadata(null, OnRecommendedActionPropertyChanged));

        static Button()
        {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
        }

        public Button()
        {
            Style = (Style)Application.Current.Resources["StandardButtonStyle"];

            Click += Button_Click;
        }

        public Thickness ImageMargin
        {
            get => (Thickness)GetValue(ImageMarginProperty);
            set => SetValue(ImageMarginProperty, value);
        }

        public ButtonStyle ButtonStyle
        {
            get => (ButtonStyle)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        public ContextMenu DropDownMenu
        {
            get => (ContextMenu)GetValue(DropDownMenuProperty);
            set => SetValue(DropDownMenuProperty, value);
        }

        public BitmapImage Image
        {
            get => (BitmapImage)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public bool IsBorderless
        {
            get => (bool)GetValue(IsBorderlessProperty);
            set => SetValue(IsBorderlessProperty, value);
        }

        public string RecommendedAction
        {
            get => (string)GetValue(RecommendedActionProperty);
            set => SetValue(RecommendedActionProperty, value);
        }

        public bool IsRecommended
        {
            get => (bool)GetValue(IsRecommendedProperty);
            set => SetValue(IsRecommendedProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        private static void OnImagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnIsRecommendedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnRecommendedActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (Button)d;
            ctl.IsRecommended = ctl.Name == (string)e.NewValue;
        }

        private static void OnBorderlessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button ctl && (bool)e.NewValue)
            {
                ctl.BorderThickness = new Thickness(0);
                ctl.Background = Brushes.Transparent;
                ctl.MinWidth = 0;
            }
        }
        
        private static void OnButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (Button)d;
            switch (e.NewValue)
            {
                case ButtonStyle.Rounded:
                    obj.Style = (Style)Application.Current.Resources["RoundedButtonStyle"];
                    break;

                case ButtonStyle.Toolbar:
                    obj.Style = (Style)Application.Current.Resources["ToolbarButtonStyle"];
                    break;

                default:
                    obj.Style = (Style)Application.Current.Resources["StandardButtonStyle"];
                    break;
            }                
        }

        private CustomPopupPlacement[] PopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            return new[] { new CustomPopupPlacement(new Point(5, 90), PopupPrimaryAxis.Vertical) };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ctxMenu = DropDownMenu;
            if (ctxMenu != null)
            {
                ctxMenu.PlacementTarget = this;
                ctxMenu.CustomPopupPlacementCallback = PopupPlacementCallback;
                ctxMenu.Placement = PlacementMode.Custom;
                ctxMenu.IsOpen = true;
            }
        }
    }
}
