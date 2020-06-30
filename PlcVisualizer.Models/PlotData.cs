using System.Windows.Media;
using Prism.Mvvm;

namespace PlcVisualizer.Models
{
    public class PlotData : BindableBase
    {
        private string _label;
        private PointRef _lastValue;
        private Brush _foreground;
        private int _maxSamples = 0;

        /// <summary>
        /// Gets or sets the latest value.
        /// </summary>
        public PointRef LastValue
        {
            get => _lastValue;
            set => SetProperty(ref _lastValue, value);
        }

        /// <summary>
        /// Gets or sets the plot label.
        /// </summary>
        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }

        /// <summary>
        /// Gets or sets max number of samples kept in the buffer. 0 means the plot control will maintain that value to fill the screen.
        /// </summary>
        public int MaxSamples
        {
            get => _maxSamples;
            set => SetProperty(ref _maxSamples, value);
        }

        /// <summary>
        /// Gets or sets the plot color.
        /// </summary>
        public Brush Foreground
        {
            get => _foreground;
            set => SetProperty(ref _foreground, value);
        }
    }
}
