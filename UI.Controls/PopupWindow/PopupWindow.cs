using System;
using System.Windows;
using System.Windows.Interop;

namespace UI.Controls;

public class PopupWindow : Window
{
    public PopupWindow(Window owner = null)
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

        Loaded += Window_Loaded;
    }

    public new WindowPos WindowPos
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

    public new bool? ShowDialog()
    {
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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // WindowIconVisibility = Icon != null ? Visibility.Visible : Visibility.Collapsed;
        var hwndHelper = new WindowInteropHelper(this);
        var monitor = Win32.MonitorFromWindow(hwndHelper.Handle, Win32.MONITOR_DEFAULTTONEAREST);
        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = Win32.GetMonitorInfo(monitor);
            Win32.RECT workArea = monitorInfo.rcWork;

            // Scale the width and height according to the zoom factor
            double zoomFactor = (double)Application.Current.Resources["ZoomFactor"];
            if (zoomFactor > 1)
            {
                Width = Math.Min(Width * zoomFactor, workArea.Width);
                Height = Math.Min(Height * zoomFactor, workArea.Height);

                if (WindowStartupLocation == WindowStartupLocation.CenterOwner && Owner != null)
                {
                    double left = Owner.WindowState == WindowState.Maximized ? workArea.Left : Owner.Left;
                    double top = Owner.WindowState == WindowState.Maximized ? workArea.Top : Owner.Top;
                    Left = left + ((Owner.Width - Width) / 2);
                    Top = top + ((Owner.Height - Height) / 2);
                }                    
            }
        }
    }
}