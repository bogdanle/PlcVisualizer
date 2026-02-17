using System.Windows;

namespace UI.Controls;

public class RadioButton : System.Windows.Controls.RadioButton
{
    public RadioButton()
    {
        Style = (Style)Application.Current.Resources["MyRadioButtonStyle"];           
    }        
}