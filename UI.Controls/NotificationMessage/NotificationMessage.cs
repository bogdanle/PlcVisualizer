using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace UI.Controls
{
    public class NotificationMessage : Control
    {
        public static readonly DependencyProperty ContentDataTemplateProperty = DependencyProperty.Register("ContentDataTemplate", typeof(DataTemplate), typeof(NotificationMessage));
        public static readonly DependencyProperty AutoFadeProperty = DependencyProperty.Register("AutoFade", typeof(bool), typeof(NotificationMessage), new PropertyMetadata(true));
        public static readonly DependencyProperty FadeoutTimeProperty = DependencyProperty.Register("FadeoutTime", typeof(Duration), typeof(NotificationMessage), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(5)), OnFadeoutTimeChanged));
        public static readonly DependencyProperty MessageDataProperty = DependencyProperty.Register("MessageData", typeof(NotificationMessageData), typeof(NotificationMessage), new PropertyMetadata(null, OnMessageDataChanged));

        internal static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(NotificationMessageData), typeof(NotificationMessage));
        internal static readonly DependencyProperty MessageTypeProperty = DependencyProperty.Register("MessageType", typeof(NotificationMessageType), typeof(NotificationMessage));
        internal static readonly DependencyProperty ResetProperty = DependencyProperty.Register("Reset", typeof(bool), typeof(NotificationMessage), new PropertyMetadata(OnResetChanged));
        internal static readonly DependencyProperty ScrollButtonsVisibilityProperty = DependencyProperty.Register("ScrollButtonsVisibility", typeof(Visibility), typeof(NotificationMessage), new PropertyMetadata(Visibility.Collapsed));
        internal static readonly DependencyProperty MessagesCountProperty = DependencyProperty.Register("MessagesCount", typeof(string), typeof(NotificationMessage));
        internal static readonly DependencyProperty CurrentMessageProperty = DependencyProperty.Register("CurrentMessage", typeof(string), typeof(NotificationMessage));
        internal static readonly RoutedEvent MessageChangedEvent = EventManager.RegisterRoutedEvent("MessageChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(NotificationMessage));

        private readonly DispatcherTimer _fadeTimer;
        private readonly List<NotificationMessageData> _messageQueue = new List<NotificationMessageData>();
        private int _currentMessageIndex = -1;
        private bool _isNotificationMessageVisible;
        private Border _contentPlaceholder;
        private Canvas _prevContentPlaceholder;

        public NotificationMessage()
        {
            _fadeTimer = new DispatcherTimer { Interval = FadeoutTime.TimeSpan };
            _fadeTimer.Tick += FadeOutTimer_Tick;
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommandExecuted));
            CommandBindings.Add(new CommandBinding(ScrollBar.LineLeftCommand, PrevMessageCommandExecuted));
            CommandBindings.Add(new CommandBinding(ScrollBar.LineRightCommand, NextMessageCommandExecuted));

            Style = (Style)Application.Current.Resources["NotificationMessageStyle"];
        }

        internal event RoutedEventHandler MessageChanged
        {
            add => AddHandler(MessageChangedEvent, value);
            remove => RemoveHandler(MessageChangedEvent, value);
        }

        public DataTemplate ContentDataTemplate
        {
            get => (DataTemplate)GetValue(ContentDataTemplateProperty);
            set => SetValue(ContentDataTemplateProperty, value);
        }

        public bool AutoFade
        {
            get => (bool)GetValue(AutoFadeProperty);
            set => SetValue(AutoFadeProperty, value);
        }

        public Duration FadeoutTime
        {
            get => (Duration)GetValue(FadeoutTimeProperty);
            set => SetValue(FadeoutTimeProperty, value);
        }

        public NotificationMessageData MessageData
        {
            get => (NotificationMessageData)GetValue(MessageDataProperty);
            set => SetValue(MessageDataProperty, value);
        }

        internal NotificationMessageData Message
        {
            get => (NotificationMessageData)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        internal NotificationMessageType MessageType
        {
            get => (NotificationMessageType)GetValue(MessageTypeProperty);
            set => SetValue(MessageTypeProperty, value);
        }

        internal bool Reset
        {
            get => (bool)GetValue(ResetProperty);
            set => SetValue(ResetProperty, value);
        }
                
        internal Visibility ScrollButtonsVisibility
        {
            get => (Visibility)GetValue(ScrollButtonsVisibilityProperty);
            set => SetValue(ScrollButtonsVisibilityProperty, value);
        }

        internal string MessagesCount
        {
            get => (string)GetValue(MessagesCountProperty);
            set => SetValue(MessagesCountProperty, value);
        }

        internal string CurrentMessage
        {
            get => (string)GetValue(CurrentMessageProperty);
            set => SetValue(CurrentMessageProperty, value);
        }

        private int CurrentMessageIndex
        {
            get => _currentMessageIndex;

            set
            {
                _currentMessageIndex = value;
                CurrentMessage = (_currentMessageIndex + 1).ToString(CultureInfo.InvariantCulture);
            }
        }

        private bool IsNotificationMessageVisible
        {
            get => _isNotificationMessageVisible;

            set
            {
                _isNotificationMessageVisible = value;

                if (_isNotificationMessageVisible)
                {
                    Visibility = Visibility.Visible;

                    if (AutoFade)
                    {
                        _fadeTimer.Stop();
                        _fadeTimer.Start();
                    }
                }
                else
                {
                    if (FindResource("FadeoutAnimation") is Storyboard storyBoard)
                    {
                        storyBoard.Begin(this);
                    }
                }
            }
        }
                
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ContentDataTemplate == null)
            {
                ContentDataTemplate = (DataTemplate)FindResource("NotificationMessageTemplate");
            }

            _contentPlaceholder = (Border)GetTemplateChild("contentPlaceholder");
            _prevContentPlaceholder = (Canvas)GetTemplateChild("prevContentPlaceholder");
        }

        public void Hide()
        {
            _fadeTimer.Stop();
            IsNotificationMessageVisible = false;
            _currentMessageIndex = -1;
            _messageQueue.Clear();
        }

        private static void OnMessageDataChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((NotificationMessage)source).OnMessageDataChanged();
        }

        private static void OnFadeoutTimeChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((NotificationMessage)source).OnFadeoutTimeChanged();
        }

        private static void OnResetChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((NotificationMessage)source).OnResetChanged();
        }

        private void SwitchMessage(NotificationMessageData messageData)
        {
            FrameworkElement content = _contentPlaceholder;
            if (content != null)
            {
                var brush = new VisualBrush(_contentPlaceholder.Child)
                    {
                        Viewbox = new Rect(0, 0, content.ActualWidth, content.ActualHeight),
                        ViewboxUnits = BrushMappingMode.Absolute,
                        Stretch = Stretch.None
                    };

                // Get the current dpi values
                var source = PresentationSource.FromVisual(this);
                var dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                var dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;

                // Convert the visual brush to a bitmap as we don't want the image to be updated every time the visual changes
                var rtb = new RenderTargetBitmap((int)(content.ActualWidth * dpiX / 96.0), (int)(content.ActualHeight * dpiX / 96.0), dpiX, dpiY, PixelFormats.Pbgra32);
                var dv = new DrawingVisual();
                using (var ctx = dv.RenderOpen())
                {
                    ctx.DrawRectangle(brush, null, brush.Viewbox);
                }

                rtb.Render(dv);

                _prevContentPlaceholder.Background = new ImageBrush { ImageSource = rtb };
                _prevContentPlaceholder.Opacity = 1;
            }

            Message = messageData;
            MessageType = messageData.Type;
            AutoFade = messageData.AutoFade;
            IsNotificationMessageVisible = true;
            RaiseEvent(new RoutedEventArgs(MessageChangedEvent));
        }

        private void OnFadeoutTimeChanged()
        {
            _fadeTimer.Interval = FadeoutTime.TimeSpan;
            _fadeTimer.Stop();
        }
        
        private void OnMessageDataChanged()
        {
            if (!string.IsNullOrEmpty(MessageData?.Message))
            {
                _messageQueue.Add(MessageData);
                Message = MessageData;
                MessageType = MessageData.Type;
                AutoFade = MessageData.AutoFade;
                CurrentMessageIndex = _messageQueue.Count - 1;
                MessagesCount = _messageQueue.Count.ToString(CultureInfo.InvariantCulture);                
                ScrollButtonsVisibility = _messageQueue.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
                IsNotificationMessageVisible = true;
            }
        }

        private void OnResetChanged()
        {
            if (Reset)
            {
                Visibility = Visibility.Collapsed;
            }
        }
        
        private void FadeOutTimer_Tick(object sender, EventArgs e)
        {            
            if (CurrentMessageIndex == _messageQueue.Count - 1)
            {
                Hide();
            }
            else
            {
                GoToNextMessage();
            }
        }

        private void CloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Hide();
        }

        private void PrevMessageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _fadeTimer.Stop();

            int prevIndex = CurrentMessageIndex;
            CurrentMessageIndex -= (CurrentMessageIndex > 0) ? 1 : 0;
            if (prevIndex != CurrentMessageIndex)
            {
                SwitchMessage(_messageQueue[CurrentMessageIndex]);
            }
        }

        private void NextMessageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _fadeTimer.Stop();

            GoToNextMessage();
        }

        private void GoToNextMessage()
        {
            int prevIndex = CurrentMessageIndex;
            CurrentMessageIndex += (CurrentMessageIndex < _messageQueue.Count - 1) ? 1 : 0;
            if (prevIndex != CurrentMessageIndex)
            {
                SwitchMessage(_messageQueue[CurrentMessageIndex]);
            }
        }
    }
}
