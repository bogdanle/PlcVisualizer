using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using UI.Infrastructure;
using UI.Infrastructure.Interfaces;
using UI.Infrastructure.Messaging;

namespace UI.Shell.ViewModels;

public partial class TopBarViewModel : ViewModelCore<TopBarViewModel>
{
    [ObservableProperty]
    private int _notificationCount;

    public TopBarViewModel(
        IMessenger messenger,
        IMessageBoxService messageBox,
        IFileDialogService fileDialog,
        IErrorDialogService errorDialog,
        ILogger<TopBarViewModel> logger)
        : base(messenger, messageBox, fileDialog, errorDialog, logger)
    {
        Messenger.Register<TopBarViewModel, UpdateNotificationCountMessage>(this, static (r, m) => r.NotificationCount = m.Value);
    }

    public string ApplicationVersion { get; set; }

    public string CurrentUser => Environment.UserName;

    [RelayCommand]
    private void ShowNotificationCenter()
    {
        Messenger.Send(new ShowNotificationCenterMessage(true));
    }
}
