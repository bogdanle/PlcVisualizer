using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace UI.Controls;
#pragma warning disable SA1305
#pragma warning disable SA1307
#pragma warning disable SA1300
#pragma warning disable SA1401
#pragma warning disable SA1201
#pragma warning disable SA1310
#pragma warning disable SA1201
#pragma warning disable SA1202

public static class Win32
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref POINT pt);

    [DllImport("user32.dll")]
    internal static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    internal static extern void Mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint WM_GETMINMAXINFO = 0x0024;
    public const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
                 
    public static void SendClick(Point point)
    {
        Mouse_event(MOUSEEVENTF_LEFTDOWN, (int)point.X, (int)point.Y, 0, UIntPtr.Zero);
        Mouse_event(MOUSEEVENTF_LEFTUP, (int)point.X, (int)point.Y, 0, UIntPtr.Zero);
    }

    public static Point GetCursorPos()
    {
        var pt = default(POINT);
        GetCursorPos(ref pt);
        return new Point(pt.X, pt.Y);
    }

    public static void SetCursorPos(Point point)
    {
        SetCursorPos((int)point.X, (int)point.Y);
    }

    public static void EnableDropShadow(bool enable = true)
    {
        const uint SPI_SETDROPSHADOW = 0x1025;
        const uint SPIF_SENDCHANGE = 0x02;
        uint flag = enable ? 1U : 0;
        SystemParametersInfo(SPI_SETDROPSHADOW, 0, ref flag, SPIF_SENDCHANGE);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public RECT rcMonitor;
        public RECT rcWork;
        public int dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int cx;
        public int cy;
    }

    public struct RECT
    {
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;

        public void Offset(int dx, int dy)
        {
            _left += dx;
            _top += dy;
            _right += dx;
            _bottom += dy;
        }

        public int Left
        {
            get => _left;
            set => _left = value;
        }

        public int Right
        {
            get => _right;
            set => _right = value;
        }

        public int Top
        {
            get => _top;
            set => _top = value;
        }

        public int Bottom
        {
            get => _bottom;
            set => _bottom = value;
        }

        public int Width => _right - _left;

        public int Height => _bottom - _top;

        public POINT Position => new POINT { X = _left, Y = _top };

        public SIZE Size => new SIZE { cx = Width, cy = Height };

        public static RECT Union(RECT rect1, RECT rect2)
        {
            return new RECT
            {
                Left = Math.Min(rect1.Left, rect2.Left),
                Top = Math.Min(rect1.Top, rect2.Top),
                Right = Math.Max(rect1.Right, rect2.Right),
                Bottom = Math.Max(rect1.Bottom, rect2.Bottom),
            };
        }
    }

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    public static MONITORINFO GetMonitorInfo(IntPtr hMonitor)
    {
        var mi = new MONITORINFO();
        if (!_GetMonitorInfo(hMonitor, mi))
        {
            throw new System.ComponentModel.Win32Exception();
        }

        return mi;
    }

    [DllImport("user32.dll", EntryPoint = "GetMonitorInfo", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool _GetMonitorInfo(IntPtr hMonitor, [In, Out] MONITORINFO lpmi);
}
#pragma warning restore SA1305
#pragma warning restore SA1307
#pragma warning restore SA1300
#pragma warning restore SA1401
#pragma warning restore SA1201
#pragma warning restore SA1310
#pragma warning restore SA1201
#pragma warning restore SA1202