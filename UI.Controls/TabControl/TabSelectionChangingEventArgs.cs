using System.Windows;

namespace UI.Controls
{
    public class TabSelectionChangingEventArgs : RoutedEventArgs
    {
        public TabSelectionChangingEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        public object CurrentItem { get; set; }
    }
}