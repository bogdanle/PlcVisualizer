using System.Windows;

namespace UI.Controls;

public class RichTextBox : System.Windows.Controls.RichTextBox
{
    static RichTextBox()
    {
        // DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
    }

    public RichTextBox()
    {
        Style = (Style)Application.Current.Resources["MyRichTextBoxStyle"];
    }
}