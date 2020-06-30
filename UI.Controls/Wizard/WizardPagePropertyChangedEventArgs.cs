namespace UI.Controls.Wizard
{
    public class WizardPagePropertyChangedEventArgs : System.EventArgs
    {
        public IWizardPageInfo PageInfo { get; set; }

        public string PropertyName { get; set; }
    }
}