using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace UI.Controls
{
    public class ComboBox : System.Windows.Controls.ComboBox
    {
        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(ComboBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty UniqueIndicatorVisibilityProperty = DependencyProperty.Register("UniqueIndicatorVisibility", typeof(Visibility), typeof(ComboBox), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty RequiredIndicatorVisibilityProperty = DependencyProperty.Register("RequiredIndicatorVisibility", typeof(Visibility), typeof(ComboBox), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty WatermarkForegroundProperty = DependencyProperty.Register("WatermarkForeground", typeof(Brush), typeof(ComboBox));
        private static readonly DependencyPropertyKey IsMouseLeftButtonDownPropertyKey = DependencyProperty.RegisterReadOnly("IsMouseLeftButtonDown", typeof(bool), typeof(ComboBox), new PropertyMetadata());
        private static readonly DependencyPropertyKey HasSelectedItemPropertyKey = DependencyProperty.RegisterReadOnly("HasSelectedItem", typeof(bool), typeof(ComboBox), new PropertyMetadata());
        private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly("HasText", typeof(bool), typeof(ComboBox), new PropertyMetadata());
        private static readonly DependencyProperty IsMouseLeftButtonDownProperty = IsMouseLeftButtonDownPropertyKey.DependencyProperty;
        private static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;
        private static readonly DependencyProperty HasSelectedItemProperty = HasSelectedItemPropertyKey.DependencyProperty;
        private System.Windows.Controls.TextBox _textBox;

        static ComboBox()
        {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));
        }

        public ComboBox() 
            : base()
        {
            Style = (Style)Application.Current.Resources["MyComboBoxStyle"];

            Grid.SetIsSharedSizeScope(this, true);
        }

        public Popup Popup { get; set; }

        public string WatermarkText
        {
            get => (string)GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        public Brush WatermarkForeground
        {
            get => (Brush)GetValue(WatermarkForegroundProperty);
            set => SetValue(WatermarkForegroundProperty, value);
        }

        public bool HasText
        {
            get => (bool)GetValue(HasTextProperty);
            private set => SetValue(HasTextPropertyKey, value);
        }

        public bool HasSelectedItem
        {
            get => (bool)GetValue(HasSelectedItemProperty);
            private set => SetValue(HasSelectedItemPropertyKey, value);
        }

        public Visibility RequiredIndicatorVisibility
        {
            get => (Visibility)GetValue(RequiredIndicatorVisibilityProperty);
            set => SetValue(RequiredIndicatorVisibilityProperty, value);
        }

        public Visibility UniqueIndicatorVisibility
        {
            get => (Visibility)GetValue(UniqueIndicatorVisibilityProperty);
            set => SetValue(UniqueIndicatorVisibilityProperty, value);
        }

        public bool IsMouseLeftButtonDown
        {
            get => (bool)GetValue(IsMouseLeftButtonDownProperty);
            private set => SetValue(IsMouseLeftButtonDownPropertyKey, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Popup = (Popup)GetTemplateChild("Popup");

            _textBox = (System.Windows.Controls.TextBox)GetTemplateChild("PART_EditableTextBox");
            if (_textBox != null)
            {
                DependencyPropertyDescriptor.FromProperty(IsFocusedProperty, typeof(System.Windows.Controls.TextBox)).AddValueChanged(_textBox, (s, e) =>
                {
                    VisualStateManager.GoToState(this, _textBox.IsFocused ? "Focused" : "Unfocused", true);
                });
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ComboBoxItem;
        }
        
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ComboBoxItem();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            HasSelectedItem = SelectedIndex != -1;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Down && !IsDropDownOpen)
            {
                IsDropDownOpen = true;
                e.Handled = true;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }
    }
}
