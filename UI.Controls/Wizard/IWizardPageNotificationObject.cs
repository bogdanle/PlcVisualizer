using System;

namespace UI.Controls.Wizard
{
    public interface IWizardPageNotificationObject
    {
        /// <summary>
        /// Raised when page status has been changed.
        /// </summary>
        event EventHandler<WizardPageStatusChangedEventArgs> PageStatusChanged;

        /// <summary>
        /// Called by the framework when the page status has been changed by another page.
        /// </summary>
        /// <param name="newStatus">The status.</param>
        void OnPageStatusChanged(WizardPageStatus newStatus);

        /// <summary>
        /// Called by the framework when the page is made the active page.
        /// </summary>
        /// <returns>True if the page was successfully set active; otherwise False.</returns>
        bool OnPageActivate();

        /// <summary>
        /// Called by the framework when the current page is no longer the active page. Perform data validation here.
        /// </summary>
        /// <param name="backward">True if navigating backwards.</param>
        /// <returns>True if data was updated successfully, otherwise False.</returns>
        bool OnPageDeactivate(bool backward);

        /// <summary>
        /// This procedure is called by the framework when the user clicks on the Finish or Cancel button in a wizard. 
        /// </summary>
        /// <param name="cancelled">True if wizard was cancelled by the user.</param>
        void OnWizardFinish(bool cancelled = false);

        bool CanGoBack();
    }
}
