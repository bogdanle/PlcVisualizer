using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using UI.Controls;
using UI.Infrastructure;
using UI.Infrastructure.Messaging;

namespace UI.Shell.Views;

/// <summary>
/// Interaction logic for NotificationCenter.xaml.
/// </summary>
public partial class NotificationCenter : UserControl
{
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(NotificationCenter), new PropertyMetadata(true, OnIsOpenChanged));
    private readonly IMessenger _messenger;
    private ICommand _clearAllCommand;

    public NotificationCenter()
    {
        InitializeComponent();

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommandExecuted));

        Focusable = true;

        _messenger = (IMessenger)AppServices.Provider.GetService(typeof(IMessenger));
        _messenger.Register<NotificationCenter, NotificationMessageEnvelope>(this, static (r, m) =>
            Application.Current.Dispatcher.Invoke(() => r.OnNotificationMessage(m.Value)));

        EventManager.RegisterClassHandler(typeof(MainWindow), Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
    }

    public ICommand ClearAllCommand => _clearAllCommand ??= new RelayCommand(OnClearAll, ClearAllCanExecute);

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
        _messenger.Send(new UpdateNotificationCountMessage(Items.Count));
    }

    private bool ClearAllCanExecute()
    {
        return Items.Count > 0;
    }

    private void OnClearAll()
    {
        Items.Clear();
        _messenger.Send(new UpdateNotificationCountMessage(Items.Count));
    }

    private void CloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        var ctl = (Control)e.OriginalSource;
        var data = (NotificationMessageData)ctl.DataContext;

        Items.Remove(data);
        _messenger.Send(new UpdateNotificationCountMessage(Items.Count));
    }
}