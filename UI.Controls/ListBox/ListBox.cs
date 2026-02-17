using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UI.Controls;

public class ListBox : System.Windows.Controls.ListBox
{        
    static ListBox()
    {
        // DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(typeof(ListBox)));
    }

    public ListBox() 
    {
        Style = (Style)Application.Current.Resources["MyListBoxStyle"];

        ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
    }

    public ScrollViewer ScrollViewer { get; private set; }

    public ItemsPresenter ItemsPresenter { get; private set; }
    
    public int FirstVisibleItem
    {
        get
        {
            if (ScrollViewer == null || ScrollViewer.ExtentHeight == 0)
            {
                return 0;
            }

            return (int)(Items.Count * ScrollViewer.VerticalOffset / ScrollViewer.ExtentHeight);
        }

        set => ScrollViewer?.ScrollToVerticalOffset((double)value / Items.Count * ScrollViewer.ExtentHeight);
    }

    public int VisibleItemCount
    {
        get
        {
            if (ScrollViewer == null || ScrollViewer.ExtentHeight == 0)
            {
                return 10;
            }

            return Math.Max(3, (int)Math.Ceiling(Items.Count * ScrollViewer.ViewportHeight / ScrollViewer.ExtentHeight));
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ScrollViewer = (ScrollViewer)GetTemplateChild("scrollViewer");
        ItemsPresenter = (ItemsPresenter)GetTemplateChild("itemsPresenter");
    }

    public void ClearSelection()
    {
        SelectedIndex = -1;
    }

    public void SelectIndex(int index)
    {
        if (index >= Items.Count)
        {
            index = Items.Count - 1;
        }

        if (index < 0)
        {
            index = 0;
        }

        SelectedIndex = index;
        ScrollIntoView(SelectedItem);
    }

    public void CenterViewOn(int index)
    {
        FirstVisibleItem = index - (VisibleItemCount / 2);
    }

    public ListBoxItem GetParentItem(UIElement element)
    {
        ListBoxItem container = null;
        while (container == null && element != null)
        {
            element = VisualTreeHelper.GetParent(element) as UIElement;
            container = element as ListBoxItem;
        }

        return container;
    }

    public ListBoxItem ItemFromPoint(Point point)
    {
        if (InputHitTest(point) is UIElement element)
        {
            return GetParentItem(element);
        }

        return null;
    }

    protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
    {
        base.OnMouseEnter(e);

        VisualStateManager.GoToState(this, "MouseOver", true);
    }

    protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        VisualStateManager.GoToState(this, "Normal", true);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);

        VisualStateManager.GoToState(this, "Focused", true);
    }

    private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
    {
        if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
        {
            foreach (object item in Items)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) != null)
                {
                    var pi = item.GetType().GetProperty("Description");
                    if (pi != null)
                    {
                        string text = (string)pi.GetValue(item, null);
                        if (text != null)
                        {
                            AutomationProperties.SetName(ItemContainerGenerator.ContainerFromItem(item), text);
                        }
                    }
                }
            }
        }
    }            
}