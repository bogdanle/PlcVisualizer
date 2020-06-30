using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace UI.Controls.Wizard
{
    public interface IWizardPageInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when page status has been changed.
        /// </summary>
        event EventHandler<WizardPageStatusChangedEventArgs> PageStatusChanged;

        event EventHandler<WizardPagePropertyChangedEventArgs> PagePropertyChanged;
      
        /// <summary>
        /// Gets or sets the unique page name (will be used later to retrieve the page from collection). It will also appear in the left-hand pane (progress ladder). 
        /// Should be kept short due to space restriction in the progress ladder pane.
        /// </summary>
        string PageName { get; set; }

        /// <summary>
        /// Gets or sets the page title, most of the time will be the same as PageName. Will be displayed in right-hand pane (page container) header area.
        /// </summary>
        string PageTitle { get; set; }

        /// <summary>
        /// Gets or sets holds the actual page's UI control (the view).
        /// </summary>
        UserControl PageView { get; set; }

        /// <summary>
        /// Gets or sets reference to a notification object (e.g. ViewModel class).
        /// </summary>
        IWizardPageNotificationObject NotificationObject { get; set; }

        /// <summary>
        /// Gets a value indicating whether indicates if page is ticked (displayed with tick mark in progress ladder).
        /// </summary>
        bool IsPageTicked { get; }

        /// <summary>
        /// Gets a value indicating whether indicates if page is crossed (displayed with a cross in progress ladder).
        /// </summary>
        bool IsPageCrossed { get; }

        /// <summary>
        /// Gets or sets a value indicating whether indicates if page is enabled or disabled (grayed out and inaccessible in progress ladder).
        /// </summary>
        bool IsPageEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether indicates if the page is selected in progress ladder.
        /// </summary>
        bool IsPageSelected { get; set; }

        /// <summary>
        /// Gets or sets the page status (Valid or Invalid).
        /// </summary>    
        WizardPageStatus PageStatus { get; set; }

        bool PageHasActivated { get; set; }
    }
}
