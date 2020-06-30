using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace UI.Controls.Wizard
{
    public class WizardPageInfo : IWizardPageInfo
    {        
        private WizardPageStatus _pageStatus = WizardPageStatus.Invalid;
        private bool _eventsSubscribed;
        private bool _isPageSelected;
        private bool _pageHasActivated;
        private bool _isPageEnabled = true;
        private string _pageTitle = string.Empty;
        private string _pageName = string.Empty;
        private INotifyPropertyChanged _notifyPropChanged;
        private IWizardPageNotificationObject _notificationObject;
        private UserControl _pageView;

        public WizardPageInfo()
        {
        }

        public WizardPageInfo(string pageName, string pageTitle, UserControl pageView = null, IWizardPageNotificationObject notificationObject = null)
        {
            _pageName = pageName;
            _pageTitle = pageTitle;
            _pageView = pageView;

            NotificationObject = notificationObject;
        }

        public event EventHandler<WizardPageStatusChangedEventArgs> PageStatusChanged;

        public event EventHandler<WizardPagePropertyChangedEventArgs> PagePropertyChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool PageHasActivated
        {
            get => _pageHasActivated;

            set
            {
                if (!Equals(value, _pageHasActivated))
                {
                    _pageHasActivated = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string PageName
        {
            get => _pageName;

            set
            {
                _pageName = value;
                RaisePropertyChanged();
            }
        }

        public string PageTitle
        {
            get => string.IsNullOrEmpty(_pageTitle) ? _pageName : _pageTitle;

            set
            {
                _pageTitle = value;
                RaisePropertyChanged();
            }
        }

        public UserControl PageView
        {
            get => _pageView;

            set
            {
                _pageView = value;
                RaisePropertyChanged();
            }
        }

        public IWizardPageNotificationObject NotificationObject
        {
            get => _notificationObject;

            set
            {
                if (value != null)
                {
                    _notificationObject = value;
                    _notificationObject.PageStatusChanged += OnPageStatusChanged;
                    _notifyPropChanged = _notificationObject as INotifyPropertyChanged;
                    SubscribeToEvents();
                }
            }
        }

        public bool IsPageTicked => IsPageEnabled && _pageStatus == WizardPageStatus.Valid && PageHasActivated;

        public bool IsPageCrossed => IsPageEnabled && _pageStatus == WizardPageStatus.Crossed;        

        public bool IsPageEnabled
        {
            get => _isPageEnabled;

            set
            {
                _isPageEnabled = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsPageTicked));
                RaisePropertyChanged(nameof(IsPageCrossed));
            }
        }

        public bool IsPageSelected
        {
            get => _isPageSelected;

            set
            {
                _isPageSelected = value;
                RaisePropertyChanged();
            }
        }

        public WizardPageStatus PageStatus
        {
            get => _pageStatus;

            set
            {
                if (_pageStatus != value)
                {
                    var args = new WizardPageStatusChangedEventArgs() { OldStatus = _pageStatus, NewStatus = value, PageInfo = this };
                    _pageStatus = value;

                    PageStatusChanged?.Invoke(this, args);

                    RaisePropertyChanged(nameof(PageStatus));
                    RaisePropertyChanged(nameof(IsPageTicked));
                    RaisePropertyChanged(nameof(IsPageCrossed));
                }
            }
        }

        private void OnPageStatusChanged(object sender, WizardPageStatusChangedEventArgs e)
        {
            PageStatus = e.NewStatus;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PagePropertyChanged?.Invoke(sender, new WizardPagePropertyChangedEventArgs() { PageInfo = this, PropertyName = e.PropertyName });
        }

        private void SubscribeToEvents()
        {
            if (!_eventsSubscribed && _notifyPropChanged != null)
            {
                _eventsSubscribed = true;

                _notifyPropChanged.PropertyChanged += OnPropertyChanged;
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
