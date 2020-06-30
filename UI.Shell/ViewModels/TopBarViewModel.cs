using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using UI.Infrastructure;
using UI.Infrastructure.Events;
using Unity;

namespace UI.Shell.ViewModels
{
    public class TopBarViewModel : ViewModelCore<TopBarViewModel>
    {
        private ICommand _showNotificationCenterCommand;
        private int _notificationCount;

        public TopBarViewModel(IUnityContainer container) 
            : base(container)
        {
            EventAggregator.GetEvent<UpdateNotificationCountEvent>().Subscribe(OnUpdateNotificationCount, ThreadOption.UIThread);
        }

        public string ApplicationVersion { get; set; }

        public int NotificationCount
        {
            get => _notificationCount;
            set => SetProperty(ref _notificationCount, value);
        }

        public string CurrentUser => Environment.UserName;

        public ICommand ShowNotificationCenterCommand => _showNotificationCenterCommand ?? (_showNotificationCenterCommand = new DelegateCommand(OnShowNotificationCenter));

        private void OnUpdateNotificationCount(int count)
        {
            NotificationCount = count;
        }

        private void OnShowNotificationCenter()
        {
            EventAggregator.GetEvent<ShowNotificationCenterEvent>().Publish(true);
        }
    }
}
