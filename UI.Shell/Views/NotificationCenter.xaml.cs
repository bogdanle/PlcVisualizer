using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;
using Prism.Events;
using UI.Controls;
using UI.Infrastructure.Events;

namespace UI.Shell.Views
{
    /// <summary>
    /// Interaction logic for NotificationCenter.xaml.
    /// </summary>
    public partial class NotificationCenter : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(NotificationCenter), new PropertyMetadata(true, OnIsOpenChanged));
        private readonly IEventAggregator _eventAggregator;
        private ICommand _clearAllCommand;

        public NotificationCenter()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommandExecuted));

            try
            {
                Focusable = true;

                _eventAggregator = null; // DiContainer.GetInstance().Resolve<IEventAggregator>();
                _eventAggregator?.GetEvent<NotificationMessageEvent>().Subscribe(OnNotificationMessage, ThreadOption.UIThread);

                EventManager.RegisterClassHandler(typeof(MainWindow), Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
            }
            catch (Exception)
            {
            }
        }

        public ICommand ClearAllCommand => _clearAllCommand ?? (_clearAllCommand = new DelegateCommand(OnClearAll, ClearAllCanExecute)); // .ObservesProperty(() => Items));

        public ObservableCollection<NotificationMessageData> Items { get; } = new ObservableCollection<NotificationMessageData>();

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (!IsKeyboardFocusWithin)
            {
                IsOpen = false;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
            {
                IsOpen = false;
            }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState((UserControl)d, (bool)e.NewValue ? "Normal" : "Collapsed", true);

            if ((bool)e.NewValue)
            {
                ((UserControl)d).Focus();
            }
        }

        private void OnPreviewMouseDown(object sender, MouseEventArgs e)
        {
            var result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (result == null && IsOpen)
            {
                IsOpen = false;
            }
        }

        private void OnNotificationMessage(NotificationMessageData data)
        {
            Items.Insert(0, data);

            _eventAggregator.GetEvent<UpdateNotificationCountEvent>().Publish(Items.Count);
        }

        private bool ClearAllCanExecute()
        {
            return Items.Count > 0;
        }

        private void OnClearAll()
        {
            Items.Clear();

            _eventAggregator.GetEvent<UpdateNotificationCountEvent>().Publish(Items.Count);
        }

        private void CloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ctl = (Control)e.OriginalSource;
            var data = (NotificationMessageData)ctl.DataContext;

            Items.Remove(data);

            _eventAggregator.GetEvent<UpdateNotificationCountEvent>().Publish(Items.Count);
        }
    }
}
