using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace UI.Controls.Wizard
{
    [TemplatePart(Name = "TitleText", Type = typeof(TextBlock))] 
    [TemplatePart(Name = "ListBox", Type = typeof(ListBox))] 
    [TemplatePart(Name = "TickIndicator", Type = typeof(TextBlock))]
    [TemplatePart(Name = "NormalItem", Type = typeof(TextBlock))]
    public class WizardProgressLadder : Control
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(WizardProgressLadder), new PropertyMetadata(null, OnItemsSourceChanged));
        private static WizardProgressLadder _this;
        private readonly Storyboard _selectionStoryboard;
        private System.Windows.Controls.ListBox _listBox;
        private TextBlock _titleText;
        private Border _selectionBorder;        
        private object _items;
        private DispatcherTimer _timer;
        private int _prevSelectedIndex = -1;

        public WizardProgressLadder()
        {
            _this = this;
            DefaultStyleKey = typeof(WizardProgressLadder);

            var da = new DoubleAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(400)) };

            Storyboard.SetTargetName(da, "selectionBorderTransform");
            Storyboard.SetTargetProperty(da, new PropertyPath("Y"));

            _selectionStoryboard = new Storyboard();
            _selectionStoryboard.Children.Add(da);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listBox = GetTemplateChild("ListBox") as System.Windows.Controls.ListBox;
            if (_listBox != null)
            {
                _listBox.ItemsSource = _items as IEnumerable;
            }

            _titleText = GetTemplateChild("TitleText") as TextBlock;
            _selectionBorder = GetTemplateChild("SelectionBorder") as Border;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Start();
            _timer.Tick += (o, e) =>
                {
                    int i = 0;
                    foreach (var item in (IEnumerable)_items)
                    {
                        if (item is IWizardPageInfo pi)
                        {
                            if (pi.IsPageEnabled == false)
                            {
                                VisualStateManager.GoToState(GetListBoxItem(i), "Disabled", true);
                            }
                            else if (pi.IsPageSelected)
                            {
                                _prevSelectedIndex = i;
                                VisualStateManager.GoToState(GetListBoxItem(i), "Selected", true);
                            }

                            i++;
                        }
                    }

                    _timer.Stop();
                };
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WizardProgressLadder ctl)
            {
                if (ctl._listBox != null)
                {
                    ctl._listBox.ItemsSource = (IEnumerable)e.NewValue;
                }
                else
                {
                    ctl._items = e.NewValue;
                }
            }

            foreach (var item in (IEnumerable)e.NewValue)
            {
                if (item is IWizardPageInfo pi)
                {
                    pi.PropertyChanged += Pi_PropertyChanged;
                }
            }
        }

        private static void Pi_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is IWizardPageInfo pi)
            {
                _this.OnPropertyChanged(e.PropertyName, pi);
            }
        }

        private ListBoxItem GetListBoxItem(int i)
        {
            return _listBox?.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
        }

        private ListBoxItem GetListBoxItem(IWizardPageInfo item)
        {
            return _listBox?.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
        }

        private int GetSelectedIndex()
        {
            int i = 0;
            foreach (var item in (IEnumerable)_items)
            {
                if (item is IWizardPageInfo pi && pi.IsPageSelected)
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        private void OnPropertyChanged(string propertyName, IWizardPageInfo pi)
        {
            switch (propertyName)
            {
                case "IsPageTicked":
                    VisualStateManager.GoToState(GetListBoxItem(pi), pi.IsPageTicked ? "Ticked" : "Unticked", true);
                    break;

                case "IsPageSelected":
                    if (_prevSelectedIndex != -1)
                    {
                        VisualStateManager.GoToState(GetListBoxItem(_prevSelectedIndex), "Unselected", true);
                    }

                    int idx = GetSelectedIndex();
                    if (idx != -1)
                    {
                        _prevSelectedIndex = idx;
                        VisualStateManager.GoToState(GetListBoxItem(idx), "Selected", true);
                    }

                    if (pi.IsPageSelected)
                    {
                        MoveSelectionBorder(GetSelectedIndex());
                    }

                    break;
            }
        }

        private void MoveSelectionBorder(int selectedIndex)
        {
            try
            {
                var lbi0 = GetListBoxItem(0);
                var lbi1 = GetListBoxItem(selectedIndex);

                GeneralTransform gt = lbi0.TransformToVisual(this);
                Point pt0 = gt.Transform(new Point(0, 0));
                
                gt = lbi1.TransformToVisual(this);
                Point pt1 = gt.Transform(new Point(0, 0));

                var da = (DoubleAnimation)_selectionStoryboard.Children[0];
                da.To = pt1.Y - pt0.Y + 4;

                _selectionStoryboard.Begin(_selectionBorder);
            }
            catch (Exception)
            {
            }
        }     
    }
}
