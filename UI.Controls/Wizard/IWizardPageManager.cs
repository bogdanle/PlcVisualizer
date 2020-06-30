using System;
using System.Collections.ObjectModel;

namespace UI.Controls.Wizard
{
    public interface IWizardPageManager
    {
        /// <summary>
        /// Raised when navigating to a new page.
        /// </summary>
        event EventHandler<WizardPageChangedEventArgs> CurrentPageChanged;

        /// <summary>
        /// Raised when current page status has been changed.
        /// </summary>
        event EventHandler<WizardPageStatusChangedEventArgs> CurrentPageStatusChanged;

        event EventHandler<WizardPagePropertyChangedEventArgs> CurrentPagePropertyChanged;

        /// <summary>
        /// Raised when adding/removing a page.
        /// </summary>
        event EventHandler<EventArgs> PageCollectionChanged;

        ObservableCollection<IWizardPageInfo> Pages { get; }

        IWizardPageInfo CurrentPage { get; }

        IWizardPageInfo SelectedPage { get; }

        WizardPageStatus CurrentPageStatus { get; }

        void InsertPage(int index, IWizardPageInfo pageInfo);

        /// <summary>
        /// Adds a new page to the wizard container.
        /// </summary>
        /// <param name="page">The page to be added.</param>
        /// <param name="notificationObject">WizardPageInfo object.</param>
        void AddPage(IWizardPageInfo page, IWizardPageNotificationObject notificationObject = null);

        /// <summary>
        /// Removes page from collection.
        /// </summary>
        /// <param name="pageName">The unique page name.</param>
        void RemovePage(string pageName);

        /// <summary>
        /// Removes page from collection.
        /// </summary>
        /// <param name="pageInfo">The page to be removed.</param>
        void RemovePage(IWizardPageInfo pageInfo);

        /// <summary>
        /// Returns the page index from given page object.
        /// </summary>
        /// <param name="pageInfo">The user control that represents the wizard page.</param>
        /// <returns>The page index or -1 if page not found.</returns>
        int IndexOf(IWizardPageInfo pageInfo);

        /// <summary>
        /// Marks page as valid/invalid. Valid page will appear as 'ticked' in progress ladder pane.
        /// </summary>
        /// <param name="pageName">The unique page name.</param>
        /// <param name="status">Page status.</param>
        void SetPageStatus(string pageName, WizardPageStatus status);

        /// <summary>
        /// Marks given page as selected in progress ladder (left-hand pane).
        /// </summary>
        /// <param name="pageName">The unique page name.</param>
        /// <param name="selected">True to select page, false otherwise.</param>
        void SelectPage(string pageName, bool selected = true);

        /// <summary>
        /// Marks given page as deselected in progress ladder (left-hand pane).
        /// </summary>
        /// <param name="pageName">The unique page name.</param>
        void UnselectPage(string pageName);

        /// <summary>
        /// Retrieves the page info form given name.
        /// </summary>
        /// <param name="pageName">The page name.</param>
        /// <returns>WizardPageInfo object if index was found in collection. Nothing (null) otherwise.</returns>
        IWizardPageInfo GetPage(string pageName);

        /// <summary>
        /// Indicates if navigation backward is possible.
        /// </summary>
        /// <returns>True if can navigate to previous page, False otherwise.</returns>
        bool CanGoBack();

        /// <summary>
        /// Indicates if navigation forward is possible.
        /// </summary>
        /// <returns>True if can navigate to next page, False otherwise.</returns>
        bool CanGoForward();

        /// <summary>
        /// Navigates to previous (enabled) page in collection.
        /// </summary>
        void GoToPreviousPage();

        /// <summary>
        /// Navigates to next (enabled) page in collection.
        /// </summary>
        void GoToNextPage();

        /// <summary>
        /// Navigates to specific page.
        /// </summary>
        /// <param name="pageIndex">Valid page index to navigate to.</param>
        void GoToPage(int pageIndex);

        /// <summary>
        /// Called when 'Finish' button was clicked.
        /// </summary>    
        void Finish();

        /// <summary>
        /// Called when the wizard was cancelled.
        /// </summary>    
        void Cancel();

        /// <summary>
        /// Reset page status for all pages after the current one.
        /// </summary>
        void ClearPagesStatus();        
    }
}
