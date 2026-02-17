using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UI.Controls;

public class TabControl : System.Windows.Controls.TabControl
{
    public static readonly RoutedEvent TabSelectionChangingEvent = EventManager.RegisterRoutedEvent("TabSelectionChanging", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TabControl));
    public static readonly RoutedEvent TabSelectionChangedEvent = EventManager.RegisterRoutedEvent("TabSelectionChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TabControl));
    public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register("ContentMargin", typeof(Thickness), typeof(TabControl), new PropertyMetadata(new Thickness(5)));
    public static readonly DependencyProperty TabStripColumnSizeProperty = DependencyProperty.Register("TabStripColumnSize", typeof(GridLength), typeof(TabControl), new PropertyMetadata(GridLength.Auto));
    public static readonly DependencyProperty ShowTabsProperty = DependencyProperty.Register("ShowTabs", typeof(bool), typeof(TabControl), new PropertyMetadata(true, OnShowTabsChanged));
    public static readonly DependencyProperty EnableTransitionsProperty = DependencyProperty.Register("EnableTransitions", typeof(bool), typeof(TabControl), new PropertyMetadata(true));
    public static readonly DependencyProperty SeparatorVisibilityProperty = DependencyProperty.Register("SeparatorVisibility", typeof(Visibility), typeof(TabControl), new PropertyMetadata(Visibility.Visible));
    private VisualBrush _prevContentBrush;
    private Border _contentPlaceholder;
    private Canvas _prevContentPlaceholder;

    public TabControl()
    {
        Style = (Style)Application.Current.Resources["HorizontalTabControlStyle"];

        DependencyPropertyDescriptor.FromProperty(TabStripPlacementProperty, typeof(TabControl)).AddValueChanged(this, (s, e) =>
        {
            Style = (Style)Application.Current.Resources[TabStripPlacement == Dock.Left ? "VerticalTabControlStyle" : "HorizontalTabControlStyle"];
        });
    }

    public event RoutedEventHandler TabSelectionChanging
    {
        add => AddHandler(TabSelectionChangingEvent, value);
        remove => RemoveHandler(TabSelectionChangingEvent, value);
    }

    public event RoutedEventHandler TabSelectionChanged
    {
        add => AddHandler(TabSelectionChangedEvent, value);
        remove => RemoveHandler(TabSelectionChangedEvent, value);
    }

    public Thickness ContentMargin
    {
        get => (Thickness)GetValue(ContentMarginProperty);
        set => SetValue(ContentMarginProperty, value);
    }

    public GridLength TabStripColumnSize
    {
        get => (GridLength)GetValue(TabStripColumnSizeProperty);
        set => SetValue(TabStripColumnSizeProperty, value);
    }

    public bool ShowTabs
    {
        get => (bool)GetValue(ShowTabsProperty);
        set => SetValue(ShowTabsProperty, value);
    }

    public bool EnableTransitions
    {
        get => (bool)GetValue(EnableTransitionsProperty);
        set => SetValue(EnableTransitionsProperty, value);
    }

    public Visibility SeparatorVisibility
    {
        get => (Visibility)GetValue(SeparatorVisibilityProperty);
        set => SetValue(SeparatorVisibilityProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _contentPlaceholder = (Border)GetTemplateChild("contentPlaceholder");
        _prevContentPlaceholder = (Canvas)GetTemplateChild("prevContentPlaceholder");
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        if (e.RemovedItems.Count > 0 && e.AddedItems.Count > 0 && e.RemovedItems[0] != e.AddedItems[0])
        {
            if (EnableTransitions)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    try
                    {
                        OnTabSelectionChanging(e);
                        OnTabSelectionChanged(e);
                    }
                    catch (Exception)
                    {
                        _prevContentBrush = null;
                    }
                }));
            }
            else
            {
                RaiseTabSelectionChangingEvent();
                base.OnSelectionChanged(e);
            }
        }
        else
        {
            base.OnSelectionChanged(e);
        }
    }

    protected void OnTabSelectionChanging(SelectionChangedEventArgs e)
    {
        if (SelectedContent != null && _prevContentBrush == null)
        {
            var content = GetTabItemVisualRoot();
            if (content != null)
            {
                _prevContentBrush =
                    new VisualBrush(content)
                    {
                        Viewbox = new Rect(0, 0, content.ActualWidth, content.ActualHeight),
                        ViewboxUnits = BrushMappingMode.Absolute,
                        Stretch = Stretch.None
                    };

                // Point pageOffset = content.TransformToVisual(this).Transform(new Point());
                // prevContentPlaceholder.Margin = new Thickness(pageOffset.X, pageOffset.Y, 0, 0);
                _prevContentPlaceholder.Background = _prevContentBrush;
                _prevContentPlaceholder.Opacity = 1;
                _prevContentBrush = null;
            }

            RaiseTabSelectionChangingEvent();
        }
        else
        {
            _prevContentBrush = null;
        }
    }

    protected void OnTabSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);

        RaiseTabSelectionChangedEvent();
    }

    private static void OnShowTabsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = d as TabControl;
        if ((bool)e.NewValue == false)
        {
            ctl.Style = (Style)Application.Current.Resources["NoTabControlStyle"];
        }
    }

    private void RaiseTabSelectionChangingEvent()
    {
        var args = new TabSelectionChangingEventArgs(TabSelectionChangingEvent) { CurrentItem = SelectedContent };
        RaiseEvent(args);
    }

    private void RaiseTabSelectionChangedEvent()
    {
        var args = new RoutedEventArgs(TabSelectionChangedEvent);
        RaiseEvent(args);
    }

    private FrameworkElement GetTabItemVisualRoot()
    {
        var content = SelectedContent as FrameworkElement;
        if (content == null)
        {
            DependencyObject tmp = _contentPlaceholder;
            for (int i = 0; i < 10; i++)
            {
                tmp = VisualTreeHelper.GetChild(tmp, 0);
                if (tmp is Panel panel)
                {
                    content = panel;
                    break;
                }
            }
        }

        return content;
    }
}