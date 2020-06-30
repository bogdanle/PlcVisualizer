using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using PlcVisualizer.Models;

namespace PlcVisualizer.Views.Controls
{
    public partial class PlotControl : Control
    {
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(PlotControl), new PropertyMetadata(null));
        public static readonly DependencyProperty MaxSamplesProperty = DependencyProperty.Register("MaxSamples", typeof(int), typeof(PlotControl), new PropertyMetadata(-1, OnMaxSamplesChangedCallback));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(PlotControl), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty LastValueProperty = DependencyProperty.Register("LastValue", typeof(PointRef), typeof(PlotControl), new PropertyMetadata(null, OnLastValueChangedCallback));
        private const double MarginInternal = 10;
        private readonly PointList _points = new PointList();
        private Border _plotBorder;
        private Grid _grid;
        private ScrollViewer _scrollViewer;

        public PlotControl()
        {
            Style = (Style)Application.Current.Resources["PlotControlStyle"];

            SizeChanged += PlotControl_SizeChanged;            
        }

        /// <summary>
        /// Gets or sets value that will be added to internal circular buffer of points and plot will be updated accordingly.
        /// </summary>
        public Point LastValue
        {
            get => (Point)GetValue(LastValueProperty);
            set => SetValue(LastValueProperty, value);
        }

        /// <summary>
        /// Gets or sets a label associated with a plot.
        /// </summary>
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public int MaxSamples
        {
            get => (int)GetValue(MaxSamplesProperty);
            set => SetValue(MaxSamplesProperty, value);
        }

        /// <summary>
        /// Gets or sets PointCollection for Poly-line object. 
        /// </summary>
        internal PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        private double ScaleX { get; set; } = 3;

        private double ScaleY { get; set; } = 1;

        private double OffsetY { get; set; } = 100;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _plotBorder = (Border)GetTemplateChild("PlotBorder");
            _grid = (Grid)GetTemplateChild("Grid");
            _scrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");

            if (_scrollViewer != null && MaxSamples == 0)
            {
                _scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                _scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        private static void OnLastValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var obj = (PlotControl)d;
            obj.OnLastValueChanged((PointRef)args.NewValue);
        }

        private static void OnMaxSamplesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var obj = (PlotControl)d;
            var val = (int)args.NewValue;
            if (val > 0)
            {
                obj._points.MaxItems = val;

                if (obj._scrollViewer != null)
                {
                    obj._scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    obj._scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
            }
            else
            {
                if (obj._scrollViewer != null)
                {
                    obj._scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    obj._scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
            }
        }

        private void PlotControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MaxSamples == 0)
            {
                // Update the circular buffer size accordingly to the size of the plot control and the x axis scale.
                _points.MaxItems = (int)(e.NewSize.Width / ScaleX);
            }

            DrawBackground();
        }

        private void OnLastValueChanged(PointRef newValue)
        {
            _points.Add(newValue);

            // Figure out the y-axis scaling based on min and max values 
            var min = _points.Min(p => p.Y);
            var max = _points.Max(p => p.Y);

            // Update the scale and offset accordingly
            ScaleY = (_plotBorder.ActualHeight - (2 * MarginInternal)) / (max - min);
            OffsetY = _plotBorder.ActualHeight - MarginInternal + (min * ScaleY);

            // Convert to point collection
            var items = new PointCollection();
            foreach (var point in _points)
            {
                items.Add(new Point(ScaleX * point.X, OffsetY - (point.Y * ScaleY)));
            }

            Points = items;
        }

        private void DrawBackground()
        {
            _grid.Children.Clear();

            _grid.RowDefinitions.Clear();
            _grid.ColumnDefinitions.Clear();

            for (int i = 0; i < (int)ActualHeight / 20; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });            
            }

            for (int i = 0; i < (int)ActualWidth / 60; i++)
            {
               _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 1; i < _grid.RowDefinitions.Count; i++)
            {
                var line = new Line() { X1 = 0, X2 = 1, Stretch = Stretch.Fill, StrokeThickness = 0.5, Stroke = Brushes.LightSteelBlue, VerticalAlignment = VerticalAlignment.Top };
                _grid.Children.Add(line);
                Grid.SetRow(line, i);
                Grid.SetColumnSpan(line, 99);
            }

            for (int i = 1; i < _grid.ColumnDefinitions.Count; i++)
            {
                var line = new Line() { Y1 = 0, Y2 = 1, Stretch = Stretch.Fill, StrokeThickness = 0.5, Stroke = Brushes.LightSteelBlue, HorizontalAlignment = HorizontalAlignment.Left };
                _grid.Children.Add(line);
                Grid.SetColumn(line, i);
                Grid.SetRowSpan(line, 99);
            }
        }

        /// <summary>
        /// Class that implements a circular buffer of points.
        /// </summary>
        public class PointList : IEnumerable<PointRef>
        {
            private readonly List<PointRef> _list = new List<PointRef>();

            public int Count => _list.Count;

            public int MaxItems { get; set; } = 250;

            public void Add(PointRef point)
            {
                // Keep adding until you reach MaxItems. Then remove the first point after adding new one.
                _list.Add(new PointRef(_list.Count, point.Y));

                if (_list.Count > MaxItems)
                {
                    _list.RemoveAt(0);

                    foreach (var item in _list)
                    {
                        item.X -= 1;
                    }
                }
            }

            public void Clear()
            {
                _list.Clear();
            }

            public IEnumerator<PointRef> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
