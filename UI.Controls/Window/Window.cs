using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace UI.Controls;

public class Window : System.Windows.Window
{
    public static readonly DependencyProperty IsGrayedOutProperty = DependencyProperty.Register(nameof(IsGrayedOut), typeof(bool), typeof(Window), new PropertyMetadata(false, OnIsGrayedOutChanged));
    public static readonly DependencyProperty LayoutRootMarginProperty = DependencyProperty.Register(nameof(LayoutRootMargin), typeof(Thickness), typeof(Window), new PropertyMetadata(new Thickness(0)));

    private CaptionButton _minimize;
    private CaptionButton _maximize;
    private CaptionButton _restore;
    private CaptionButton _close;
    private Grid _layoutRoot;
    private CaptionButtonType _captionButtons = CaptionButtonType.Close | CaptionButtonType.Maximize | CaptionButtonType.Minimize;        
    private bool _sizing;
    private bool _moving;
    private Point _startPos;
        
    public Window()
    {
        Style = (Style)Application.Current.Resources["MyWindowStyle"];

        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanRestoreWindow));            

        Loaded += Window_Loaded;            
    }

    public bool IsGrayedOut
    {
        get => (bool)GetValue(IsGrayedOutProperty);
        set => SetValue(IsGrayedOutProperty, value);
    }

    public Thickness LayoutRootMargin
    {
        get => (Thickness)GetValue(LayoutRootMarginProperty);
        set => SetValue(LayoutRootMarginProperty, value);
    }

    public WindowPos WindowPos
    {
        get => new WindowPos { Left = Left, Top = Top, Width = Width, Height = Height, State = WindowState };

        set
        {
            if (value != null)
            {
                Left = value.Left;
                Top = value.Top;
                Width = value.Width;
                Height = value.Height;
                WindowState = value.State;
            }
        }
    }

    public CaptionButtonType CaptionButtons
    {
        get => _captionButtons;

        set
        {
            _captionButtons = value;
            UpdateCaptionButtons();
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _layoutRoot = (Grid)GetTemplateChild("LayoutRoot");

        var resizeGrip = (ResizeGrip)GetTemplateChild("WindowResizeGrip");
        if (resizeGrip != null)
        {
            resizeGrip.MouseLeftButtonDown += ResizeGrip_MouseLeftButtonDown;
        }

        _minimize = (CaptionButton)GetTemplateChild("Minimize");
        _maximize = (CaptionButton)GetTemplateChild("Maximize");
        _restore = (CaptionButton)GetTemplateChild("Restore");
        _close = (CaptionButton)GetTemplateChild("Close");

        UpdateCaptionButtons();
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);

        if (WindowState == WindowState.Maximized)
        {
            _maximize.Visibility = Visibility.Collapsed;
            _restore.Visibility = Visibility.Visible;

            _layoutRoot.Margin = LayoutRootMargin;
        }
        else
        {
            _maximize.Visibility = Visibility.Visible;
            _restore.Visibility = Visibility.Collapsed;

            _layoutRoot.Margin = new Thickness(0);
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

            var newWidth = Width;
            newWidth += newPos.X - _startPos.X;
            if (newWidth >= 0)
            {
                Width = newWidth;
            }

            var newHeight = Height;
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

    private static void OnIsGrayedOutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var obj = (Window)d;
        VisualStateManager.GoToState(obj, (bool)e.NewValue ? "GrayedOut" : "Normal", true);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
        var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
        mainWindowSrc?.AddHook(WndProc);
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

    private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr param)
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

    private void UpdateCaptionButtons()
    {
        if (_close != null)
        {
            _close.Visibility = (_captionButtons & CaptionButtonType.Close) == CaptionButtonType.Close ? Visibility.Visible : Visibility.Collapsed;
            _restore.Visibility = (_captionButtons & CaptionButtonType.Restore) == CaptionButtonType.Restore ? Visibility.Visible : Visibility.Collapsed;
            _minimize.Visibility = (_captionButtons & CaptionButtonType.Minimize) == CaptionButtonType.Minimize ? Visibility.Visible : Visibility.Collapsed;
            _maximize.Visibility = (_captionButtons & CaptionButtonType.Maximize) == CaptionButtonType.Maximize ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void ResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        CaptureMouse();
        _startPos = e.GetPosition(this);

        _sizing = true;
        Cursor = Cursors.SizeNWSE;
    }
      
    private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
    }

    private void OnCanRestoreWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = WindowState == WindowState.Maximized;
    }

    private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode != ResizeMode.NoResize;
    }
      
    private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MaximizeWindow(this);
    }

    private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.RestoreWindow(this);
    }
}