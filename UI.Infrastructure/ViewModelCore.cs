using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using UI.Controls;
using UI.Infrastructure.Interfaces;
using UI.Infrastructure.Messaging;

namespace UI.Infrastructure;

/// <summary>
/// Common infrastructure for view-model classes.
/// </summary>
/// <typeparam name="TViewModelType">The ViewModel type.</typeparam>
public abstract partial class ViewModelCore<TViewModelType> : ObservableObject, IDataErrorInfo
    where TViewModelType : ObservableObject
{
    private readonly Dictionary<string, string> _errors = new();

    protected ViewModelCore(
        IMessenger messenger,
        IMessageBoxService messageBox,
        IFileDialogService fileDialog,
        IErrorDialogService errorDialog,
        ILogger<TViewModelType> logger)
    {
        Messenger = messenger;
        MessageBox = messageBox;
        FileDialog = fileDialog;
        ErrorDialog = errorDialog;
        Logger = logger;
    }

    public bool EnableUpdates { get; set; }

    public bool IsModified
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsBusy
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                Messenger.Send(new ShowBusyIndicatorMessage(value));
            }
        }
    }

    public bool IsBusyWithProgress
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                Messenger.Send(new ShowBusyIndicatorWithProgressMessage(value));
            }
        }
    }

    public bool HasErrors
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string Title { get; set; }

    public string Error { get; } = string.Empty;

    public bool IsValid { get; protected set; }

    public virtual bool KeepAlive => true;

    public string RecommendedAction
    {
        get;
        set => SetProperty(ref field, value);
    }

    public dynamic View
    {
        get;
        set
        {
            field = value;
            OnViewChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the underlying view is active (visible).
    /// </summary>
    public bool IsActive
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnActivated(value);
            }
        }
    }

    public bool ToolbarStripConnected { get; set; }

    protected dynamic ToolbarStrip { get; set; }

    protected IMessageBoxService MessageBox { get; }

    protected IFileDialogService FileDialog { get; set; }

    protected IErrorDialogService ErrorDialog { get; }

    protected ILogger<TViewModelType> Logger { get; set; }

    protected IMessenger Messenger { get; }

    public string this[string columnName] => OnValidate(columnName);

    [RelayCommand]
    public virtual void Refresh()
    {
        OnRefresh();
    }

    public void ShowNotification(string message, bool autoFade = true)
    {
        Logger.LogInformation(message);
        Messenger.Send(new NotificationMessageEnvelope(new NotificationMessageData(message, NotificationMessageType.Info) { AutoFade = autoFade }));
    }

    public void ShowNotification(string title, string message, bool autoFade = true)
    {
        Logger.LogInformation(message);
        Messenger.Send(new NotificationMessageEnvelope(new NotificationMessageData(title, message, NotificationMessageType.Info) { AutoFade = autoFade }));
    }

    public void ShowNotificationWarning(string message, bool autoFade = true)
    {
        Logger.LogWarning(message);
        Messenger.Send(new NotificationMessageEnvelope(new NotificationMessageData(message, NotificationMessageType.Warning) { AutoFade = autoFade }));
    }

    public void ShowNotificationSuccess(string message, bool autoFade = true)
    {
        Logger.LogInformation(message);
        Messenger.Send(new NotificationMessageEnvelope(new NotificationMessageData(message, NotificationMessageType.Success) { AutoFade = autoFade }));
    }

    public void ShowNotificationWarning(string title, string message, bool autoFade = true)
    {
        Logger.LogWarning(message);
        Messenger.Send(new NotificationMessageEnvelope(new NotificationMessageData(title, message, NotificationMessageType.Warning) { AutoFade = autoFade }));
    }

    public void ShowNotificationError(string message, bool autoFade = true)
    {
        Logger.LogError(message);
        Messenger.Send(new NotificationMessageEnvelope(new NotificationMessageData(message) { AutoFade = autoFade }));
    }

    public void ShowNotificationError(string title, string message, bool autoFade = true)
    {
        Logger.LogError(message);
        Messenger.Send(new NotificationMessageEnvelope(new NotificationMessageData(title, message) { AutoFade = autoFade }));
    }

    public void AddError(string propertyName, string errorMessage)
    {
        _errors[propertyName] = errorMessage;
    }

    public virtual void OnRefresh()
    {
    }

    protected virtual void OnViewModelInitialize(Type param)
    {
    }

    protected virtual void OnViewChanged()
    {
    }

    /// <summary>
    /// Called when the view model becomes active (the underlying view has been made visible).
    /// </summary>
    /// <param name="isActive">True if active, false otherwise.</param>
    protected virtual void OnActivated(bool isActive)
    {
    }

    protected virtual bool CanClose()
    {
        if (IsModified)
        {
            var result = MessageBox.Show("You have unsaved changes.\nAre you sure you want to quit the application and discard changes?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            return result == MessageBoxResult.Yes;
        }

        return true;
    }

    protected virtual bool SaveChangesPrompt()
    {
        if (IsModified)
        {
            var result = MessageBox.Show("You have unsaved changes.\nDo you want to save your changes before reloading the data?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            return result == MessageBoxResult.Yes;
        }

        return false;
    }

    protected virtual string OnValidate(string propertyName)
    {
        string error = string.Empty;
        object value = GetType().GetProperty(propertyName)?.GetValue(this, null);

        var results = new List<ValidationResult>(1);
        var context = new ValidationContext(this, null, null) { MemberName = propertyName };

        bool result = Validator.TryValidateProperty(value, context, results);
        if (!result)
        {
            var validationResult = results.First();
            error = validationResult.ErrorMessage;
        }
        else
        {
            _errors.Remove(propertyName);
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            if (!_errors.TryAdd(propertyName, error))
            {
                _errors[propertyName] = error;
            }
        }

        HasErrors = _errors.Count > 0;

        return error;
    }
}
