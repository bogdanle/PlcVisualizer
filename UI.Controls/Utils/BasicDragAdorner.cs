using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace UI.Controls;

public class BasicDragAdorner : Adorner
{
    private readonly VisualBrush _vbrush;
    private readonly Size _size;
    private Point _offset;
    private Point _location;
        
    public BasicDragAdorner(UIElement adornedElement, Size size, Point offset)
        : base(adornedElement)
    {
        IsHitTestVisible = false;

        _offset = offset;
        _size = size;
        _vbrush = new VisualBrush(adornedElement) { Opacity = 0.5 };
    }

    public void UpdatePosition(Point pos)
    {
        _location = pos;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        var pt = _location;
        pt.Offset(-_offset.X, -_offset.Y);
        dc.DrawRectangle(_vbrush, null, new Rect(pt, _size));
    }
}