using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace UI.Shell.Controls;

public partial class SidebarMenu : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SidebarMenu), new PropertyMetadata(null, OnItemsSourceChanged));

    public SidebarMenu()
    {            
        InitializeComponent();
    }

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
        
    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var obj = (SidebarMenu)d;
    }
}