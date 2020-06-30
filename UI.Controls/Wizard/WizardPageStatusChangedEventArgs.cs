namespace UI.Controls.Wizard
{
    public class WizardPageStatusChangedEventArgs : System.EventArgs
    {
        public IWizardPageInfo PageInfo { get; set; }

        public WizardPageStatus OldStatus { get; set; }

        public WizardPageStatus NewStatus { get; set; }
    }
}
