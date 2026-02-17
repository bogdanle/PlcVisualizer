using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace UI.Controls;

public class TabItem : System.Windows.Controls.TabItem
{
    private TextBlock _content;

    public TabItem()
    {
        Style = (Style)Application.Current.Resources["HorizontalTabItemStyle"];                    

        DependencyPropertyDescriptor.FromProperty(TabStripPlacementProperty, typeof(TabItem)).AddValueChanged(this, (s, e) =>
        {
            Style = (Style)Application.Current.Resources[TabStripPlacement == Dock.Left ? "VerticalTabItemStyle" : "HorizontalTabItemStyle"];                    
        });
    }
        
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _content = GetTemplateChild("Content") as TextBlock;
        if (_content != null)
        {
        }
    }
}