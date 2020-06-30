using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace UI.Controls.Wizard
{
    public class WizardPageManager : IWizardPageManager, INotifyPropertyChanged
    {
        private bool _eventsSubscribed;
        private int _currentIndex = -1;

        public WizardPageManager()
        {
            SubscribeToEvents();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<WizardPageChangedEventArgs> CurrentPageChanged;

        public event EventHandler<WizardPageStatusChangedEventArgs> CurrentPageStatusChanged;

        public event EventHandler<WizardPagePropertyChangedEventArgs> CurrentPagePropertyChanged;

        public event EventHandler<EventArgs> PageCollectionChanged;

        public ObservableCollection<IWizardPageInfo> Pages { get; private set; } = new ObservableCollection<IWizardPageInfo>();

        public IWizardPageInfo CurrentPage => IsIndexValid(_currentIndex) ? Pages[_currentIndex] : null;

        public IWizardPageInfo SelectedPage => Pages.FirstOrDefault(pageInfo => pageInfo.IsPageSelected);    

        public WizardPageStatus CurrentPageStatus => CurrentPage?.PageStatus ?? WizardPageStatus.Invalid;

        public void AddPage(IWizardPageInfo page, IWizardPageNotificationObject notificationObject = null)
        {
            if (page != null)
            {
                if (GetPageIndex(page.PageName) == -1)
                {
                    Pages.Add(page);

                    page.PageStatusChanged += OnPageStatusChanged;
                    page.PagePropertyChanged += OnPagePropertyChanged;

                    if (notificationObject != null)
                    {
                        page.NotificationObject = notificationObject;
                    }

                    // If this is first page we insert, navigate to it
                    if (_currentIndex == -1)
                    {
                        GoToPage(0);
                    }
                }
            }
        }

        public void InsertPage(int index, IWizardPageInfo page)
        {
            if (page != null && GetPageIndex(page.PageName) == -1)
            {
                Pages.Insert(index, page);
            }
        }

        public void RemovePage(string pageName)
        {
            int pageIndex = GetPageIndex(pageName);
            if (pageIndex != -1)
            {
                Pages[pageIndex].PageStatusChanged -= OnPageStatusChanged;
                Pages[pageIndex].PagePropertyChanged -= OnPagePropertyChanged;

                Pages.RemoveAt(pageIndex);

                // If the page about to be removed is the current page, navigate to previous one
                if (_currentIndex == pageIndex)
                {
                    GoToPage(_currentIndex - 1);
                }
            }
        }

        public void RemovePage(IWizardPageInfo pageInfo)
        {
            int pageIndex = IndexOf(pageInfo);
            if (pageIndex != -1)
            {
                Pages[pageIndex].PageStatusChanged -= OnPageStatusChanged;
                Pages[pageIndex].PagePropertyChanged -= OnPagePropertyChanged;

                Pages.RemoveAt(pageIndex);

                // If the page about to be removed is the current page, navigate to previous one
                if (_currentIndex == pageIndex)
                {
                    GoToPage(_currentIndex - 1);
                }
            }
        }

        public int IndexOf(IWizardPageInfo pageInfo)
        {
            return Pages.IndexOf(pageInfo);
        }

        public void SetPageStatus(string pageName, WizardPageStatus status)
        {
            int pageIndex = GetPageIndex(pageName);
            if (pageIndex != -1)
            {
                Pages[pageIndex].PageStatus = status;
            }
        }

        // Reset page status for all pages after the current one
        public void ClearPagesStatus()
        {
            for (int i = _currentIndex + 1; i < Pages.Count; i++)
            {
                Pages[i].PageStatus = WizardPageStatus.Invalid;
            }
        }

        public void SelectPage(string pageName, bool selected = true)
        {
            if (selected && SelectedPage != null)
            {
                SelectedPage.IsPageSelected = false;
            }

            int pageIndex = GetPageIndex(pageName);
            if (pageIndex != -1)
            {
                Pages[pageIndex].IsPageSelected = selected;
            }
        }

        public void UnselectPage(string pageName)
        {
            SelectPage(pageName, false);
        }

        public IWizardPageInfo GetPage(string pageName)
        {
            return Pages.FirstOrDefault(p => p.PageName == pageName);
        }

        public virtual bool CanGoBack()
        {
            bool result = _currentIndex > 0 && Pages.Count > 0;
            if (result)
            {
                result &= CurrentPage.NotificationObject.CanGoBack();
            }

            return result;
        }

        public virtual bool CanGoForward()
        {
            return _currentIndex < Pages.Count - 1 & Pages.Count > 0;
        }

        public void GoToPreviousPage()
        {
            int pageIndex = _currentIndex;
            while (CanGoBack(pageIndex))
            {
                pageIndex -= 1;
                if (Pages[pageIndex].IsPageEnabled)
                {
                    GoToPage(pageIndex);
                    break;
                }
            }
        }

        public void GoToNextPage()
        {
            int pageIndex = _currentIndex;
            while (CanGoForward(pageIndex))
            {
                pageIndex += 1;
                if (Pages[pageIndex].IsPageEnabled)
                {
                    GoToPage(pageIndex);
                    break;
                }
            }
        }

        public void GoToPage(int pageIndex)
        {
            if (IsIndexValid(pageIndex))
            {
                var e = new WizardPageChangedEventArgs();
                if (IsIndexValid(_currentIndex))
                {
                    e.OldPage = Pages[_currentIndex];
                }

                e.NewPage = Pages[pageIndex];

                if (CurrentPage == null || NotifyPageDeactivate(_currentIndex - pageIndex > 0))
                {
                    var selPage = SelectedPage;
                    if (selPage != null)
                    {
                        selPage.IsPageSelected = false;
                    }

                    _currentIndex = pageIndex;
                    Pages[_currentIndex].IsPageSelected = true;

                    if (NotifyPageActivate())
                    {
                        CurrentPageChanged?.Invoke(this, e);
                    }
                }
            }
        }

        public void Finish()
        {
            OnWizardFinish();
        }

        public void Cancel()
        {
            OnWizardCancel();
        }

        protected bool IsIndexValid(int pageIndex)
        {
            return pageIndex >= 0 & pageIndex < Pages.Count;
        }

        protected virtual void OnPageChanged()
        {
        }

        protected virtual void OnWizardFinish()
        {
            foreach (var page in Pages)
            {
                page.NotificationObject?.OnWizardFinish();
            }
        }

        protected virtual void OnWizardCancel()
        {
            foreach (var page in Pages)
            {
                page.NotificationObject?.OnWizardFinish();
            }
        }

        private void OnPageCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PageCollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnPageStatusChanged(object sender, WizardPageStatusChangedEventArgs e)
        {
            // Forward the event if the current page status has been updated
            if (e.PageInfo == CurrentPage)
            {
                CurrentPageStatusChanged?.Invoke(this, e);
            }
            else
            {
                NotifyPageStatusChanged(e.PageInfo, e.NewStatus);
            }
        }

        private void OnPagePropertyChanged(object sender, WizardPagePropertyChangedEventArgs e)
        {
            // Forward the event if the current page status has been updated
            if (e.PageInfo == CurrentPage)
            {
                CurrentPagePropertyChanged?.Invoke(this, e);
            }
        }

        private void RaisePropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private bool CanGoBack(int pageIndex)
        {
            return pageIndex > 0 && Pages.Count > 0;
        }

        private bool CanGoForward(int pageIndex)
        {
            return pageIndex < Pages.Count - 1 & Pages.Count > 0;
        }

        private void NotifyPageStatusChanged(IWizardPageInfo page, WizardPageStatus newStatus)
        {
            page.NotificationObject?.OnPageStatusChanged(newStatus);
        }

        private bool NotifyPageDeactivate(bool backward)
        {
            return CurrentPage.NotificationObject?.OnPageDeactivate(backward) ?? true;
        }

        private bool NotifyPageActivate()
        {
            if (CurrentPage.NotificationObject != null)
            {
                CurrentPage.PageHasActivated = true;
                return CurrentPage.NotificationObject.OnPageActivate();
            }

            return true;
        }

        private int GetPageIndex(string pageName)
        {
            return Pages.Select((page, index) => new { page, index }).FirstOrDefault(p => p.page.PageName == pageName)?.index ?? -1;
        }
     
        private void SubscribeToEvents()
        {
            if (!_eventsSubscribed)
            {
                _eventsSubscribed = true;

                Pages.CollectionChanged += OnPageCollectionChanged;
            }
        }
    }
}
