namespace PlcVisualizer.Models
{
    /// <summary>
    /// Reference-type point. The .NET Point is a value type so it has usual problems related to value-type objects.
    /// </summary>
    public class PointRef
    {
        public PointRef(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
