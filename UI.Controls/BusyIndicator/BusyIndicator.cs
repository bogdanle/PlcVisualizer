using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UI.Controls
{
    /// <summary>
    /// A control to provide a visual indicator when an application is busy.
    /// </summary>
    [TemplateVisualState(Name = VisualStates.StateIdle, GroupName = VisualStates.GroupBusyStatus)]
    [TemplateVisualState(Name = VisualStates.StateBusy, GroupName = VisualStates.GroupBusyStatus)]
    [TemplateVisualState(Name = VisualStates.StateVisible, GroupName = VisualStates.GroupVisibility)]
    [TemplateVisualState(Name = VisualStates.StateHidden, GroupName = VisualStates.GroupVisibility)]
    [StyleTypedProperty(Property = "OverlayStyle", StyleTargetType = typeof(Rectangle))]
    [StyleTypedProperty(Property = "ProgressBarStyle", StyleTargetType = typeof(ProgressBar))]
    public class BusyIndicator : ContentControl
    {
        public static readonly DependencyProperty ProgressBarVisibilityProperty = DependencyProperty.Register("ProgressBarVisibility", typeof(Visibility), typeof(BusyIndicator), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ProgressBarValueProperty = DependencyProperty.Register("ProgressBarValue", typeof(int), typeof(BusyIndicator), new PropertyMetadata(-1, OnProgressBarValueChanged));
        public static readonly DependencyProperty PercentCompleteStringProperty = DependencyProperty.Register("PercentCompleteString", typeof(string), typeof(BusyIndicator), new PropertyMetadata(null));
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false, OnIsBusyChanged));
        public static readonly DependencyProperty BusyContentProperty = DependencyProperty.Register("BusyContent", typeof(object), typeof(BusyIndicator), new PropertyMetadata(null));
        public static readonly DependencyProperty ProgressBarStyleProperty = DependencyProperty.Register("ProgressBarStyle", typeof(Style), typeof(BusyIndicator), new PropertyMetadata(null));
        public static readonly DependencyProperty OverlayStyleProperty = DependencyProperty.Register("OverlayStyle", typeof(Style), typeof(BusyIndicator), new PropertyMetadata(null));
        public static readonly DependencyProperty DisplayAfterProperty = DependencyProperty.Register("DisplayAfter", typeof(TimeSpan), typeof(BusyIndicator), new PropertyMetadata(TimeSpan.FromSeconds(0.1)));
        public static readonly DependencyProperty BusyContentTemplateProperty = DependencyProperty.Register("BusyContentTemplate", typeof(DataTemplate), typeof(BusyIndicator), new PropertyMetadata(null));
        private readonly DispatcherTimer _displayAfterTimer = new DispatcherTimer();
        private ProgressBar _progressBar;
        private Storyboard _busyStoryboard;       

        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator), new FrameworkPropertyMetadata(typeof(BusyIndicator)));
        }

        public BusyIndicator()
        {
            _displayAfterTimer.Tick += DisplayAfterTimerElapsed;
        }

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public string PercentCompleteString
        {
            get => (string)GetValue(PercentCompleteStringProperty);
            set => SetValue(PercentCompleteStringProperty, value);
        }

        public TimeSpan DisplayAfter
        {
            get => (TimeSpan)GetValue(DisplayAfterProperty);
            set => SetValue(DisplayAfterProperty, value);
        }

        public Style OverlayStyle
        {
            get => (Style)GetValue(OverlayStyleProperty);
            set => SetValue(OverlayStyleProperty, value);
        }

        public DataTemplate BusyContentTemplate
        {
            get => (DataTemplate)GetValue(BusyContentTemplateProperty);
            set => SetValue(BusyContentTemplateProperty, value);
        }

        public Visibility ProgressBarVisibility
        {
            get => (Visibility)GetValue(ProgressBarVisibilityProperty);
            set => SetValue(ProgressBarVisibilityProperty, value);
        }

        public Style ProgressBarStyle
        {
            get => (Style)GetValue(ProgressBarStyleProperty);
            set => SetValue(ProgressBarStyleProperty, value);
        }

        public int ProgressBarValue
        {
            get => (int)GetValue(ProgressBarValueProperty);
            set => SetValue(ProgressBarValueProperty, value);
        }

        public object BusyContent
        {
            get => GetValue(BusyContentProperty);
            set => SetValue(BusyContentProperty, value);
        }

        protected bool IsContentVisible { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ChangeVisualState(false);

            _progressBar = (ProgressBar)GetTemplateChild("ProgressBar");
            _busyStoryboard = (Storyboard)GetTemplateChild("BusyStoryboard");
        }

        protected virtual void OnProgressBarValueChanged(DependencyPropertyChangedEventArgs e)
        {
            int value = (int)e.NewValue;
            if (value >= 0)
            {
                // ProgressBarVisibility = Visibility.Visible;
                PercentCompleteString = $"{value}%";

                if (_progressBar != null)
                {
                    _progressBar.Value = value;
                }
            }
        }

        protected virtual void OnIsBusyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsBusy)
            {
                if (DisplayAfter.Equals(TimeSpan.Zero))
                {
                    IsContentVisible = true;
                }
                else
                {
                    _displayAfterTimer.Interval = DisplayAfter;
                    _displayAfterTimer.Start();
                }
            }
            else
            {
                _displayAfterTimer.Stop();
                IsContentVisible = false;
            }

            ChangeVisualState(true);

            if (IsBusy == false && _busyStoryboard != null)
            {
                // busyStoryboard.Begin(this, true);
                // busyStoryboard.Stop(this);
            }
        }

        protected virtual void ChangeVisualState(bool useTransitions)
        {            
            VisualStateManager.GoToState(this, IsContentVisible ? VisualStates.StateVisible : VisualStates.StateHidden, useTransitions);
        }

        private static void OnProgressBarValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BusyIndicator)d).OnProgressBarValueChanged(e);
        }

        private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BusyIndicator)d).OnIsBusyChanged(e);
        }
       
        private void DisplayAfterTimerElapsed(object sender, EventArgs e)
        {
            _displayAfterTimer.Stop();
            IsContentVisible = true;
            ChangeVisualState(true);            
        }     
    }
}
