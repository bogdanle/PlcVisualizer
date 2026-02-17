using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PlcVisualizer.Models;

public class PlotData : ObservableObject
{
    /// <summary>
    /// Gets or sets the latest value.
    /// </summary>
    public PointRef LastValue
    {
        get;
        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Gets or sets the plot label.
    /// </summary>
    public string Label
    {
        get;
        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Gets or sets max number of samples kept in the buffer. 0 means the plot control will maintain that value to fill the screen.
    /// </summary>
    public int MaxSamples
    {
        get;
        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Gets or sets the plot color.
    /// </summary>
    public Brush Foreground
    {
        get;
        set => SetProperty(ref field, value);
    }
}
