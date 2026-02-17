namespace PlcVisualizer.Models;

/// <summary>
/// Reference-type point. The .NET Point is a value type so it has usual problems related to value-type objects.
/// </summary>
public class PointRef(double x, double y)
{
    public double X { get; set; } = x;

    public double Y { get; set; } = y;
}
