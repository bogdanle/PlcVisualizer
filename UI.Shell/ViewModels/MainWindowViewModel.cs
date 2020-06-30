using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using PlcVisualizer.ViewModels;
using PlcVisualizer.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using UI.Controls;
using UI.Infrastructure;
using UI.Infrastructure.Events;
using UI.Shell.Views;
using Unity;

namespace UI.Shell.ViewModels
{
    public class MainWindowViewModel : ViewModelCore<MainWindowViewModel>
    {
        private readonly Dictionary<string, Type> _viewMap = new Dictionary<string, Type>();
        private readonly ThemeManager _themeManager;
        private bool _isBusy;
        private int _progressValue;
        private Visibility _progressBarVisibility = Visibility.Collapsed;
        private NotificationMessageData _messageData;
        private ICommand _printCommand;
        private ICommand _hideNotificationCenterCommand;
        private ICommand _openMenuCommand;
        private bool _isNotificationCenterOpen;        
        private bool _isHamburgerMenuOpen;
        private string _moduleName = "PLC Visualizer";
        private string _viewTitle = "simulation mode";

        public MainWindowViewModel(IRegionManager regionManager, IUnityContainer container) 
            : base(container)
        {
            regionManager.RegisterViewWithRegion("TopbarRegion", () => container.Resolve(typeof(TopBar)));
            regionManager.RegisterViewWithRegion("ToolbarStripRegion", () => container.Resolve(typeof(ToolbarStrip)));

            EventAggregator.GetEvent<NotificationMessageEvent>().Subscribe(OnNotificationMessage, ThreadOption.UIThread);
            EventAggregator.GetEvent<ShowBusyIndicatorEvent>().Subscribe(OnShowBusyIndicator, ThreadOption.UIThread);
            EventAggregator.GetEvent<ShowBusyIndicatorWithProgressEvent>().Subscribe(OnShowBusyIndicatorWithProgress, ThreadOption.UIThread);
            EventAggregator.GetEvent<UpdateProgressEvent>().Subscribe(OnUpdateProgress, ThreadOption.UIThread);
            EventAggregator.GetEvent<ShowNotificationCenterEvent>().Subscribe(OnShowNotificationCenter, ThreadOption.UIThread);            
            EventAggregator.GetEvent<PopupActiveEvent>().Subscribe(OnPopupActive, ThreadOption.UIThread);

            var eventAggregator = container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(OnColorThemeChanged, ThreadOption.UIThread);

            _themeManager = new ThemeManager();
            _themeManager.Apply("Cyan");

            regionManager.RegisterViewWithRegion("ContentRegion", typeof(PlcView));
            regionManager.RegisterViewWithRegion("ToolbarStripContentRegion", typeof(ToolbarStripContent));
            EventAggregator.GetEvent<InitializeViewModelEvent>().Publish(typeof(PlcViewModel));
        }

        public ICommand PrintCommand => _printCommand ?? (_printCommand = new DelegateCommand(OnPrint));
       
        public ICommand OpenMenuCommand => _openMenuCommand ?? (_openMenuCommand = new DelegateCommand(OnOpenMenu));
    
        public ICommand HideNotificationCenterCommand => _hideNotificationCenterCommand ?? (_hideNotificationCenterCommand = new DelegateCommand(OnHideNotificationCenter));

        public new bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string ModuleName
        {
            get => _moduleName;
            set => SetProperty(ref _moduleName, value);
        }

        public string ViewTitle
        {
            get => _viewTitle;
            set => SetProperty(ref _viewTitle, value);
        }

        public bool IsNotificationCenterOpen
        {
            get => _isNotificationCenterOpen;
            set => SetProperty(ref _isNotificationCenterOpen, value);
        }

        public bool IsHamburgerMenuOpen
        {
            get => _isHamburgerMenuOpen;
            set => SetProperty(ref _isHamburgerMenuOpen, value);
        }

        public Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility;
            set => SetProperty(ref _progressBarVisibility, value);
        }

        public int ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        public NotificationMessageData MessageData
        {
            get => _messageData;
            set => SetProperty(ref _messageData, value);
        }
      
        private bool IsPopupActive { get; set; }

        public void OnClosing()
        {            
        }

        private void OnColorThemeChanged(Theme obj)
        {
            _themeManager.Apply(obj.Name);
        }

        private void OnPrint()
        {
            EventAggregator.GetEvent<PrintEvent>().Publish();
        }

        private void OnOpenMenu()
        {
            IsHamburgerMenuOpen = !IsHamburgerMenuOpen;
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

        private void OnUpdateProgress(int value)
        {
            ProgressValue = value;
        }

        private void OnShowBusyIndicatorWithProgress(bool show)
        {
            if (!IsPopupActive)
            {
                IsBusy = show;
                ProgressBarVisibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void OnHideNotificationCenter()
        {
            IsNotificationCenterOpen = false;
        }

        private void OnPopupActive(bool active)
        {
            IsPopupActive = active;
        }
    }
}