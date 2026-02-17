using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using UI.Controls;
using UI.Infrastructure;
using UI.Infrastructure.Interfaces;
using UI.Infrastructure.Messaging;

namespace UI.Shell.ViewModels;

public partial class MainWindowViewModel : ViewModelCore<MainWindowViewModel>
{
    private readonly ThemeManager _themeManager;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private int _progressValue;

    [ObservableProperty]
    private Visibility _progressBarVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private NotificationMessageData _messageData;

    [ObservableProperty]
    private bool _isNotificationCenterOpen;

    [ObservableProperty]
    private bool _isHamburgerMenuOpen;

    [ObservableProperty]
    private string _moduleName = "PLC Visualizer";

    [ObservableProperty]
    private string _viewTitle = "simulation mode";

    public MainWindowViewModel(
        IMessenger messenger,
        IMessageBoxService messageBox,
        IFileDialogService fileDialog,
        IErrorDialogService errorDialog,
        ILogger<MainWindowViewModel> logger)
        : base(messenger, messageBox, fileDialog, errorDialog, logger)
    {
        _themeManager = new ThemeManager();
        _themeManager.Apply("Cyan");

        Messenger.Register<MainWindowViewModel, NotificationMessageEnvelope>(this, static (r, m) =>
            Application.Current.Dispatcher.Invoke(() => r.OnNotificationMessage(m.Value)));
        Messenger.Register<MainWindowViewModel, ShowBusyIndicatorMessage>(this, static (r, m) =>
            Application.Current.Dispatcher.Invoke(() => r.OnShowBusyIndicator(m.Value)));
        Messenger.Register<MainWindowViewModel, ShowBusyIndicatorWithProgressMessage>(this, static (r, m) =>
            Application.Current.Dispatcher.Invoke(() => r.OnShowBusyIndicatorWithProgress(m.Value)));
        Messenger.Register<MainWindowViewModel, UpdateProgressMessage>(this, static (r, m) =>
            Application.Current.Dispatcher.Invoke(() => r.ProgressValue = m.Value));
        Messenger.Register<MainWindowViewModel, ShowNotificationCenterMessage>(this, static (r, m) =>
            Application.Current.Dispatcher.Invoke(() => r.OnShowNotificationCenter(m.Value)));
        Messenger.Register<MainWindowViewModel, PopupActiveMessage>(this, static (r, m) =>
            Application.Current.Dispatcher.Invoke(() => r.IsPopupActive = m.Value));
        Messenger.Register<MainWindowViewModel, ThemeChangedMessage>(this, static (r, m) =>
            Application.Current.Dispatcher.Invoke(() => r.OnColorThemeChanged(m.Value)));
    }

    private bool IsPopupActive { get; set; }

    public void OnClosing()
    {
    }

    [RelayCommand]
    private void Print()
    {
        Messenger.Send(new PrintRequestedMessage());
    }

    [RelayCommand]
    private void OpenMenu()
    {
        IsHamburgerMenuOpen = !IsHamburgerMenuOpen;
    }

    [RelayCommand]
    private void HideNotificationCenter()
    {
        IsNotificationCenterOpen = false;
    }

    private void OnColorThemeChanged(Theme obj)
    {
        _themeManager.Apply(obj.Name);
    }

    private void OnShowNotificationCenter(bool show)
    {
        IsNotificationCenterOpen = !IsNotificationCenterOpen;
    }

    private void OnNotificationMessage(NotificationMessageData data)
    {
        if (!IsPopupActive)
        {
            MessageData = data;
        }
    }

    private void OnShowBusyIndicator(bool show)
    {
        if (!IsPopupActive)
        {
            IsBusy = show;
        }
    }

    private void OnShowBusyIndicatorWithProgress(bool show)
    {
        if (!IsPopupActive)
        {
            IsBusy = show;
            ProgressBarVisibility = show ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
