using System.Windows;

namespace UI.Infrastructure.Interfaces
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string message);
        
        MessageBoxResult Show(string message, MessageBoxButton button);

        MessageBoxResult Show(string message, MessageBoxButton button, MessageBoxImage icon);
    }
}
