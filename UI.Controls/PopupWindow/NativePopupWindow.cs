using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace UI.Controls;

public class NativePopupWindow : System.Windows.Window
{
    public static readonly DependencyProperty LayoutRootMarginProperty = DependencyProperty.Register(nameof(LayoutRootMargin), typeof(Thickness), typeof(PopupWindow), new PropertyMetadata(new Thickness(0)));

    private bool _sizing;
    private bool _moving;
    private Point _startPos;
    private CaptionButton _minimize;
    private CaptionButton _maximize;
    private CaptionButton _restore;
    private CaptionButton _close;
    private Grid _layoutRoot;
    private CaptionButtonType _captionButtons = CaptionButtonType.Close;

    static NativePopupWindow()
    {
        // DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
    }

    public NativePopupWindow()
    {
        // We'll get an exception if we try to display the window before MainWindow was shown. In that case, center on the screen.
        try
        {
            if (this != Application.Current.MainWindow)
            {
                Owner = Application.Current.MainWindow;
            }
        }
        catch (Exception e)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Console.WriteLine(e.Message);                
        }
            
        Style = (Style)Application.Current.Resources["MyPopupWindowStyle"];

        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanRestoreWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, OnShowSystemMenu, OnCanShowSystemMenu));

        Loaded += Window_Loaded;
        Closed += Window_Closed;
    }

    public NativePopupWindow(Window owner)
    {
        try
        {
            // Set the default owner to the app main window (if possible)
            if (owner == null && Application.Current != null && Application.Current.MainWindow != this)
            {
                Owner = Application.Current.MainWindow;
            }
            else
            {
                Owner = owner;
            }

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        catch (Exception)
        {
            // Ignore
        }

        Style = (Style)Application.Current?.Resources["MyPopupWindowStyle"];

        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow, CloseWindowCanExecute));
        CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanRestoreWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, OnShowSystemMenu, OnCanShowSystemMenu));

        Loaded += Window_Loaded;
        Closed += Window_Closed;
    }

    public bool CanClose { get; set; } = true;

    public CaptionButtonType CaptionButtons
    {
        get => _captionButtons;

        set
        {
            _captionButtons = value;
            UpdateCaptionButtons();
        }
    }

    public Thickness LayoutRootMargin
    {
        get => (Thickness)GetValue(LayoutRootMarginProperty);
        set => SetValue(LayoutRootMarginProperty, value);
    }

    public new bool? ShowDialog()
    {
        // Set the Owner to null if it's set to SplashScreen in order to prevent the window from disappearing when SplashScreen closes.
        if (Owner is SplashScreen)
        {
            Owner = null;
        }

        var wnd = Owner as Window;
        if (wnd != null)
        {
            wnd.IsGrayedOut = true;
        }

        bool? retVal = false;
        try
        {
            retVal = base.ShowDialog();
        }
        catch (Exception ex)
        {
            ErrorDialog.Show(ex);
        }

        if (wnd != null)
        {
            wnd.IsGrayedOut = false;
        }

        return retVal;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _layoutRoot = (Grid)GetTemplateChild("LayoutRoot");

        var captionBorder = (Border)GetTemplateChild("CaptionBorder");
        if (captionBorder != null)
        {
            captionBorder.MouseRightButtonUp += CaptionBorder_MouseRightButtonUp;
            captionBorder.MouseLeftButtonDown += CaptionBorder_MouseLeftButtonDown;
        }

        var resizeGrip = (ResizeGrip)GetTemplateChild("WindowResizeGrip");
        if (resizeGrip != null)
        {
            resizeGrip.MouseLeftButtonDown += ResizeGrip_MouseLeftButtonDown;
        }

        _minimize = (CaptionButton)GetTemplateChild("Minimize");
        _maximize = (CaptionButton)GetTemplateChild("Maximize");
        _restore = (CaptionButton)GetTemplateChild("Restore");
        _close = (CaptionButton)GetTemplateChild("Close");
        _close.IsEnabled = CanClose;

        UpdateCaptionButtons();
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        string text = AutomationProperties.GetName(this);
        if (string.IsNullOrEmpty(text))
        {
            text = Title;
        }
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);

        _layoutRoot.Margin = WindowState == WindowState.Maximized ? LayoutRootMargin : new Thickness(0);
    }

    protected void WmGetMinMaxInfo(IntPtr hwnd, IntPtr param)
    {
        var mmi = (Win32.MINMAXINFO)Marshal.PtrToStructure(param, typeof(Win32.MINMAXINFO));

        // Adjust the maximized size and position to fit the work area of the correct monitor
        var monitor = Win32.MonitorFromWindow(hwnd, Win32.MONITOR_DEFAULTTONEAREST);

        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = Win32.GetMonitorInfo(monitor);
            Win32.RECT workArea = monitorInfo.rcWork;
            Win32.RECT monitorArea = monitorInfo.rcMonitor;
            mmi.ptMaxPosition.X = Math.Abs(workArea.Left - monitorArea.Left);
            mmi.ptMaxPosition.Y = Math.Abs(workArea.Top - monitorArea.Top);
            mmi.ptMaxSize.X = Math.Abs(workArea.Right - workArea.Left);
            mmi.ptMaxSize.Y = Math.Abs(workArea.Bottom - workArea.Top);
            mmi.ptMinTrackSize.X = (int)MinWidth;
            mmi.ptMinTrackSize.Y = (int)MinHeight;

            Marshal.StructureToPtr(mmi, param, true);

            LayoutRootMargin = monitorArea.Height > workArea.Height ? new Thickness(0) : new Thickness(7);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_moving)
        {
            Point newPos = e.GetPosition(this);

            Left += newPos.X - _startPos.X;
            Top += newPos.Y - _startPos.Y;
        }
        else if (_sizing)
        {
            Point newPos = e.GetPosition(this);

            double newWidth = Width;
            newWidth += newPos.X - _startPos.X;
            if (newWidth >= 0)
            {
                Width = newWidth;
            }

            double newHeight = Height;
            newHeight += newPos.Y - _startPos.Y;
            if (newHeight >= 0)
            {
                Height = newHeight;
            }

            _startPos = newPos;
        }
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        if (_moving)
        {
            ReleaseMouseCapture();
            _moving = false;
        }
        else if (_sizing)
        {
            ReleaseMouseCapture();
            _sizing = false;

            Cursor = Cursors.Arrow;
        }
    }

    protected override void OnIsMouseCapturedChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnIsMouseCapturedChanged(e);

        if (_moving)
        {
            ReleaseMouseCapture();
            _moving = false;
        }
        else if (_sizing)
        {
            Cursor = Cursors.Arrow;
            ReleaseMouseCapture();
            _sizing = false;
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
        var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
        mainWindowSrc?.AddHook(WndProc);
    }

    private void CloseWindowCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanClose;
    }

    private void Window_Closed(object sender, EventArgs eventArgs)
    {
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr param1, IntPtr param2, ref bool handled)
    {
        if (msg == Win32.WM_GETMINMAXINFO)
        {
            handled = true;
            WmGetMinMaxInfo(hwnd, param2);
        }

        return IntPtr.Zero;
    }

    private void UpdateCaptionButtons()
    {
        if (_close != null)
        {
            _close.Visibility = ((_captionButtons & CaptionButtonType.Close) == CaptionButtonType.Close) ? Visibility.Visible : Visibility.Collapsed;
            _restore.Visibility = ((_captionButtons & CaptionButtonType.Restore) == CaptionButtonType.Restore) ? Visibility.Visible : Visibility.Collapsed;
            _minimize.Visibility = ((_captionButtons & CaptionButtonType.Minimize) == CaptionButtonType.Minimize) ? Visibility.Visible : Visibility.Collapsed;
            _maximize.Visibility = ((_captionButtons & CaptionButtonType.Maximize) == CaptionButtonType.Maximize) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
        
    private void ResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        CaptureMouse();
        _startPos = e.GetPosition(this);

        _sizing = true;
        Cursor = Cursors.SizeNWSE;
    }

    private void CaptionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            CaptureMouse();
            _startPos = e.GetPosition(this);

            _moving = true;
        }
        else if (e.ClickCount == 2)
        {
            _moving = false;
            ReleaseMouseCapture();
        }
    }

    private void CaptionBorder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
    }

    private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode != ResizeMode.NoResize;
    }

    private void OnCanShowSystemMenu(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
    {
        Close();
    }

    private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
    {
        WindowState = WindowState.Maximized;
        _maximize.Visibility = Visibility.Collapsed;
        _restore.Visibility = Visibility.Visible;
    }

    private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void OnCanRestoreWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = WindowState == WindowState.Maximized;
    }

    private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
    {
        WindowState = WindowState.Normal;

        _maximize.Visibility = Visibility.Visible;
        _restore.Visibility = Visibility.Collapsed;
    }

    private void OnShowSystemMenu(object target, ExecutedRoutedEventArgs e)
    {
    }
}