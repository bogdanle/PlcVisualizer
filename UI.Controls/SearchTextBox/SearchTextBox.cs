using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace UI.Controls
{
    public enum SearchMode 
    {
        Instant,
        Delayed,
    }

    public class SearchTextBox : System.Windows.Controls.TextBox 
    {
        public static readonly RoutedEvent SearchEvent = EventManager.RegisterRoutedEvent("Search", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchTextBox));
        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(SearchTextBox), new PropertyMetadata(string.Empty, OnWatermarkTextChanged));
        public static readonly DependencyProperty WatermarkForegroundProperty = DependencyProperty.Register("WatermarkForeground", typeof(Brush), typeof(SearchTextBox));
        public static readonly DependencyProperty SearchModeProperty = DependencyProperty.Register("SearchMode", typeof(SearchMode), typeof(SearchTextBox), new PropertyMetadata(SearchMode.Instant));
        public static readonly DependencyPropertyKey IsMouseLeftButtonDownPropertyKey = DependencyProperty.RegisterReadOnly("IsMouseLeftButtonDown", typeof(bool), typeof(SearchTextBox), new PropertyMetadata());
        public static readonly DependencyProperty IsMouseLeftButtonDownProperty = IsMouseLeftButtonDownPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly("HasText", typeof(bool), typeof(SearchTextBox), new PropertyMetadata());
        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;
        public static readonly DependencyProperty SearchEventTimeDelayProperty = DependencyProperty.Register(
                "SearchEventTimeDelay", 
                typeof(Duration), 
                typeof(SearchTextBox),
                new FrameworkPropertyMetadata(new Duration(new TimeSpan(0, 0, 0, 0, 500)), new PropertyChangedCallback(OnSearchEventTimeDelayChanged)));

        private readonly DispatcherTimer _searchEventDelayTimer;
        private string _lastReadWord;
        private string _lastKey;
        private DateTime _timeStamp;

        static SearchTextBox() 
        {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTextBox), new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }

        public SearchTextBox()
        {
            _searchEventDelayTimer = new DispatcherTimer { Interval = SearchEventTimeDelay.TimeSpan };
            _searchEventDelayTimer.Tick += OnSearchEventDelayTimerTick;

            Style = (Style)Application.Current.Resources["MySearchTextBoxStyle"];
        }

        public event RoutedEventHandler Search
        {
            add => AddHandler(SearchEvent, value);
            remove => RemoveHandler(SearchEvent, value);
        }

        public string WatermarkText
        {
            get => (string)GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        public Brush WatermarkForeground
        {
            get => (Brush)GetValue(WatermarkForegroundProperty);
            set => SetValue(WatermarkForegroundProperty, value);
        }

        public SearchMode SearchMode
        {
            get => (SearchMode)GetValue(SearchModeProperty);
            set => SetValue(SearchModeProperty, value);
        }

        public bool HasText
        {
            get => (bool)GetValue(HasTextProperty);
            private set => SetValue(HasTextPropertyKey, value);
        }

        public Duration SearchEventTimeDelay
        {
            get => (Duration)GetValue(SearchEventTimeDelayProperty);
            set => SetValue(SearchEventTimeDelayProperty, value);
        }

        public bool IsMouseLeftButtonDown
        {
            get => (bool)GetValue(IsMouseLeftButtonDownProperty);
            private set => SetValue(IsMouseLeftButtonDownPropertyKey, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_SearchIconBorder") is Border iconBorder)
            {
                iconBorder.MouseLeftButtonDown += IconBorder_MouseLeftButtonDown;
                iconBorder.MouseLeftButtonUp += IconBorder_MouseLeftButtonUp;
                iconBorder.MouseLeave += IconBorder_MouseLeave;
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            HasText = Text.Length != 0;

            if (SearchMode == SearchMode.Instant)
            {
                _searchEventDelayTimer.Stop();
                _searchEventDelayTimer.Start();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            TimeSpan timeSpan = default(TimeSpan);
            if (_timeStamp != default(DateTime))
            {
                timeSpan = DateTime.Now - _timeStamp;
            }

            _timeStamp = DateTime.Now;

            if (timeSpan > default(TimeSpan) && timeSpan < TimeSpan.FromMilliseconds(50))
            {
                return;
            }

            if (HasText)
            {
                if (e.Key == Key.Space || e.Key == Key.Return)
                {
                    var words = Text.Split(' ');
                    if (words.Length > 1)
                    {
                        var lastWord = words[words.Length - 2];

                        if (lastWord != _lastReadWord)
                        {
                            _lastReadWord = lastWord;
                        }
                    }
                }
                else if (e.Key >= Key.A && e.Key <= Key.Z)
                {
                    _lastKey = e.Key.ToString();
                }
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    _lastKey = (e.Key - Key.NumPad0).ToString();
                }
                else if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    _lastKey = (e.Key - Key.D0).ToString();
                }
            }
            else
            {
                _lastReadWord = null;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && SearchMode == SearchMode.Instant)
            {
                Text = string.Empty;
            }
            else if ((e.Key == Key.Return || e.Key == Key.Enter) && SearchMode == SearchMode.Delayed)
            {
                e.Handled = true;
                RaiseSearchEvent();
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        private static void OnSearchEventTimeDelayChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is SearchTextBox stb)
            {
                stb._searchEventDelayTimer.Interval = ((Duration)e.NewValue).TimeSpan;
                stb._searchEventDelayTimer.Stop();
            }
        }

        private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void OnSearchEventDelayTimerTick(object o, EventArgs e) 
        {
            _searchEventDelayTimer.Stop();
            RaiseSearchEvent();
        }
        
        private void IconBorder_MouseLeftButtonDown(object obj, MouseButtonEventArgs e) 
        {
            IsMouseLeftButtonDown = true;
        }

        private void IconBorder_MouseLeftButtonUp(object obj, MouseButtonEventArgs e) 
        {
            if (IsMouseLeftButtonDown)
            {
                if (HasText && SearchMode == SearchMode.Instant)
                {
                    Text = string.Empty;
                }

                if (HasText && SearchMode == SearchMode.Delayed)
                {
                    RaiseSearchEvent();
                }

                IsMouseLeftButtonDown = false;
            }
        }

        private void IconBorder_MouseLeave(object obj, MouseEventArgs e) 
        {
            IsMouseLeftButtonDown = false;
        }

        private void RaiseSearchEvent() 
        {
            var args = new RoutedEventArgs(SearchEvent);
            RaiseEvent(args);
        }
    }
}
