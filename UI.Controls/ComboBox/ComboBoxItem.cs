using System.Windows;

namespace UI.Controls;

public class ComboBoxItem : System.Windows.Controls.ComboBoxItem
{
    public ComboBoxItem()
    {
        Style = (Style)Application.Current.Resources["MyComboBoxItemStyle"];            
    }
}