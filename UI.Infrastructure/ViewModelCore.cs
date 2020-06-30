using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using UI.Controls;
using UI.Infrastructure.Events;
using UI.Infrastructure.Interfaces;
using Unity;

namespace UI.Infrastructure
{
    /// <summary>
    /// Common infrastructure for view-model classes.
    /// </summary>
    /// <typeparam name="TViewModelType">The ViewModel type.</typeparam>
    public abstract class ViewModelCore<TViewModelType> : BindableBase, INavigationAware, IRegionMemberLifetime, IDataErrorInfo
        where TViewModelType : BindableBase
    {
        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();        
        private bool _isBusy;
        private bool _hasErrors;
        private bool _isModified;
        private ICommand _refreshCommand;
        private bool _isBusyWithProgress;
        private bool _isActive;
        private dynamic _view;
        private string _recommendedAction;

        protected ViewModelCore(IUnityContainer container)
        {
            Container = container;
            MessageBox = container.Resolve<IMessageBoxService>();
            FileDialog = container.Resolve<IFileDialogService>();
            ErrorDialog = container.Resolve<IErrorDialogService>();
            EventAggregator = container.Resolve<IEventAggregator>();
            Logger = container.Resolve<ILogger>();            

            Dispatcher = Dispatcher.CurrentDispatcher;

            EventAggregator.GetEvent<ConnectToViewModelEvent<TViewModelType>>().Subscribe(OnConnectToViewModel);
            EventAggregator.GetEvent<InitializeViewModelEvent>().Subscribe(OnViewModelInitialize, ThreadOption.UIThread);
        }

        public bool EnableUpdates { get; set; }

        public bool IsModified
        {
            get => _isModified;
            set => SetProperty(ref _isModified, value);
        }

        public bool IsBusy
        {
            get => _isBusy;

            set
            {
                _isBusy = value;
                EventAggregator.GetEvent<ShowBusyIndicatorEvent>().Publish(value);
            }
        }

        public bool IsBusyWithProgress
        {
            get => _isBusyWithProgress;

            set
            {
                _isBusyWithProgress = value;
                EventAggregator.GetEvent<ShowBusyIndicatorWithProgressEvent>().Publish(value);
            }
        }

        public bool HasErrors
        {
            get => _hasErrors;
            set => SetProperty(ref _hasErrors, value);
        }

        public string Title { get; set; }

        public string Error { get; }

        public bool IsValid { get; protected set; }

        public virtual bool KeepAlive => true;

        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(OnRefresh));

        public IEventAggregator EventAggregator { get; }

        public string RecommendedAction
        {
            get => _recommendedAction;
            set => SetProperty(ref _recommendedAction, value);
        }

        public dynamic View
        {
            get => _view;

            set
            {
                _view = value;
                OnViewChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the underlying view is active (visible).
        /// </summary>
        public bool IsActive
        {
            get => _isActive;

            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnActivated(value);
                }
            }
        }

        public bool ToolbarStripConnected { get; set; }

        protected dynamic ToolbarStrip { get; set; }

        protected Dispatcher Dispatcher { get; }

        protected IMessageBoxService MessageBox { get; }

        protected IFileDialogService FileDialog { get; set; }

        protected IErrorDialogService ErrorDialog { get; }

        protected IUnityContainer Container { get; }

        protected ILogger Logger { get; set; }
       
        public string this[string columnName] => OnValidate(columnName);
    
        public void ShowNotification(string message, bool autoFade = true)
        {
            Logger.Info(message);
            EventAggregator.GetEvent<NotificationMessageEvent>().Publish(new NotificationMessageData(message, NotificationMessageType.Info) { AutoFade = autoFade });
        }

        public void ShowNotification(string title, string message, bool autoFade = true)
        {
            Logger.Info(message);
            EventAggregator.GetEvent<NotificationMessageEvent>().Publish(new NotificationMessageData(title, message, NotificationMessageType.Info) { AutoFade = autoFade });
        }

        public void ShowNotificationWarning(string message, bool autoFade = true)
        {
            Logger.Warning(message);
            EventAggregator.GetEvent<NotificationMessageEvent>().Publish(new NotificationMessageData(message, NotificationMessageType.Warning) { AutoFade = autoFade });
        }

        public void ShowNotificationSuccess(string message, bool autoFade = true)
        {
            Logger.Warning(message);
            EventAggregator.GetEvent<NotificationMessageEvent>().Publish(new NotificationMessageData(message, NotificationMessageType.Success) { AutoFade = autoFade });
        }

        public void ShowNotificationWarning(string title, string message, bool autoFade = true)
        {
            Logger.Warning(message);
            EventAggregator.GetEvent<NotificationMessageEvent>().Publish(new NotificationMessageData(title, message, NotificationMessageType.Warning) { AutoFade = autoFade });
        }

        public void ShowNotificationError(string message, bool autoFade = true)
        {
            Logger.Error(message);
            EventAggregator.GetEvent<NotificationMessageEvent>().Publish(new NotificationMessageData(message) { AutoFade = autoFade });
        }

        public void ShowNotificationError(string title, string message, bool autoFade = true)
        {
            Logger.Error(message);
            EventAggregator.GetEvent<NotificationMessageEvent>().Publish(new NotificationMessageData(title, message) { AutoFade = autoFade });
        }

        public void AddError(string propertyName, string errorMessage)
        {
            _errors[propertyName] = errorMessage;
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsActive = true;
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            IsActive = false;
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
            object value = GetType().GetProperty(propertyName).GetValue(this, null);

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
                if (_errors.ContainsKey(propertyName))
                {
                    _errors.Remove(propertyName);
                }
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                if (_errors.ContainsKey(propertyName))
                {
                    _errors[propertyName] = error;
                }
                else
                {
                    _errors.Add(propertyName, error);
                }
            }

            HasErrors = _errors.Count > 0;

            return error;
        }

        private void OnConnectToViewModel(dynamic obj)
        {
            if (!ToolbarStripConnected)
            {
                ToolbarStrip = obj;
                obj.DataContext = this;
                ToolbarStripConnected = true;
            }
        }
    }
}