namespace UI.Controls.Wizard
{
    public class WizardPageChangedEventArgs : System.EventArgs
    {
        public IWizardPageInfo OldPage { get; set; }

        public IWizardPageInfo NewPage { get; set; }
    }
}