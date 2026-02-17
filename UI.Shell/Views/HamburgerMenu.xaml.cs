using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UI.Infrastructure;
using UI.Shell.ViewModels;

namespace UI.Shell.Views;

public partial class HamburgerMenu : UserControl
{
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(HamburgerMenu), new PropertyMetadata(true, OnIsOpenChanged));

    public HamburgerMenu()
    {
        InitializeComponent();
        DataContext = (HamburgerMenuViewModel)AppServices.Provider.GetService(typeof(HamburgerMenuViewModel));

        EventManager.RegisterClassHandler(typeof(MainWindow), Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));

        Focusable = true;

        Loaded += HamburgerMenu_Loaded;
    }

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        if (!IsKeyboardFocusWithin)
        {
            IsOpen = false;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {
            IsOpen = false;
        }
    }

    private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (UserControl)d;
        bool isOpen = (bool)e.NewValue;

        VisualStateManager.GoToState(ctl, isOpen ? "Expanded" : "Collapsed", true);

        if (isOpen)
        {
            ctl.Focus();
        }
    }

    private void OnPreviewMouseDown(object sender, MouseEventArgs e)
    {
        var result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
        if (result == null && IsOpen)
        {
            IsOpen = false;
        }
    }

    private void HamburgerMenu_Loaded(object sender, RoutedEventArgs e)
    {
        dynamic vm = DataContext;
        vm.View = this;
    }
}