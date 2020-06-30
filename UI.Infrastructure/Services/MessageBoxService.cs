using UI.Infrastructure.Interfaces;

namespace UI.Infrastructure.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public System.Windows.MessageBoxResult Show(string message)
        {
            return Show(message, System.Windows.MessageBoxButton.OK);
        }

        public System.Windows.MessageBoxResult Show(string message, System.Windows.MessageBoxButton button)
        {
            return System.Windows.MessageBox.Show(message, "MESSAGE", button, System.Windows.MessageBoxImage.Information);      
        }

        public System.Windows.MessageBoxResult Show(string message, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon)
        {
            return System.Windows.MessageBox.Show(message, "MESSAGE", button, icon);
        }
    }
}
