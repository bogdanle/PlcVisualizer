using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI.Controls
{
    public enum TextBoxInputType
    {
        [Description("All")]
        All,
        [Description("Integer")]
        Integer,
        [Description("Double")]
        Double,
        [Description("Alpha")]
        Alpha,
        [Description("Alpha-numeric")]
        AlphaNumeric,
        [Description("Alpha-numeric uppercase")]
        AlphaNumericAllCaps,
        [Description("E-mail")]
        Email,
        [Description("Alpha uppercase")]
        AllCaps,
        [Description("Money")]
        Financial,
        [Description("Postcode")]
        Postcode,
        [Description("Phone number")]
        PhoneNumber,
        [Description("Percentage")]
        Percentage,
        [Description("Custom format")]
        CustomFormat        
    }

    public enum AutoCompleteMode
    {
        Append,
        Suggest,
        SuggestAppend,
        None
    }

    public class TextBox : System.Windows.Controls.TextBox
    {
        public static readonly RoutedEvent IconClickedEvent = EventManager.RegisterRoutedEvent("IconClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBox));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(TextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
        public static readonly DependencyProperty IconNameProperty = DependencyProperty.Register("IconName", typeof(string), typeof(TextBox), new PropertyMetadata(null));
        public static readonly DependencyProperty ReadOnlyForegroundProperty = DependencyProperty.Register("ReadOnlyForeground", typeof(Brush), typeof(TextBox), new PropertyMetadata(null));
        public static readonly DependencyProperty RequiredIndicatorVisibilityProperty = DependencyProperty.Register("RequiredIndicatorVisibility", typeof(Visibility), typeof(TextBox), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty UniqueIndicatorVisibilityProperty = DependencyProperty.Register("UniqueIndicatorVisibility", typeof(Visibility), typeof(TextBox), new PropertyMetadata(Visibility.Collapsed));
        public static new readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TextBox), new PropertyMetadata(false, new PropertyChangedCallback(OnIsReadOnlyChanged)));
        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(TextBox), new PropertyMetadata(string.Empty, OnWatermarkTextChanged));
        public static readonly DependencyProperty WatermarkForegroundProperty = DependencyProperty.Register("WatermarkForeground", typeof(Brush), typeof(TextBox));
        public static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly("HasText", typeof(bool), typeof(TextBox), new PropertyMetadata());
        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey IsMouseLeftButtonDownPropertyKey = DependencyProperty.RegisterReadOnly("IsMouseLeftButtonDown", typeof(bool), typeof(TextBox), new PropertyMetadata());
        public static readonly DependencyProperty IsMouseLeftButtonDownProperty = IsMouseLeftButtonDownPropertyKey.DependencyProperty;
        public static readonly DependencyProperty InputTypeProperty = DependencyProperty.Register("InputType", typeof(TextBoxInputType), typeof(TextBox), new PropertyMetadata(TextBoxInputType.All, OnInputTypeChanged));
        public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register("IconVisibility", typeof(Visibility), typeof(TextBox), new PropertyMetadata(Visibility.Collapsed, OnIconVisibilityChanged));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TextBox), new PropertyMetadata(null, OnIconChanged));
        public static readonly DependencyProperty InputPatternProperty = DependencyProperty.Register("InputPattern", typeof(string), typeof(TextBox), new PropertyMetadata(null));
        public static readonly DependencyProperty UpDownButtonVisibilityProperty = DependencyProperty.Register("UpDownButtonVisibility", typeof(Visibility), typeof(TextBox), new PropertyMetadata(Visibility.Collapsed));
        public static readonly RoutedUICommand UpDownButtonUpCommand = new RoutedUICommand();
        public static readonly RoutedUICommand UpDownButtonDownCommand = new RoutedUICommand();

        private const string FinancialPattern = "[^0-9\\-\\.,]";
        private const string PercentagePattern = "[^0-9\\.]";
        private const string UnrestrictedPercentagePattern = "[^0-9\\.-]";
        private TextBlock _currencySign;
        private string _lastReadWord;
        private string _lastKey;
        private Rectangle _iconOverlay;
        private Border _iconBorder;
        private TranslateTransform _iconBorderTransform;
        private int? _maxValue;
        private int? _minValue;
        private DateTime _timeStamp;
        private bool _enableUpdates = true;
        private Key _lastKeyPressed;

        static TextBox()
        {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        }

        public TextBox()
        {
            Style = (Style)Application.Current.Resources["MyTextBoxStyle"];

            SetupCommandBindings();
        }

        public event RoutedEventHandler IconClicked
        {
            add => AddHandler(IconClickedEvent, value);
            remove => RemoveHandler(IconClickedEvent, value);
        }

        public List<string> AutoCompleteItemsSource { get; set; }

        public AutoCompleteMode AutoCompleteMode { get; set; }

        public object Value
        {
            get => GetValue(ValueProperty);

            set
            {
                if (Value != null && value != null && Value.ToString() != value.ToString())
                {
                    SetValue(ValueProperty, value);
                }
                else
                {
                    SetValue(ValueProperty, value);
                }
            }
        }

        public string InputPattern
        {
            get => (string)GetValue(InputPatternProperty);
            set => SetValue(InputPatternProperty, value);
        }

        public Visibility UpDownButtonVisibility
        {
            get => (Visibility)GetValue(UpDownButtonVisibilityProperty);
            set => SetValue(UpDownButtonVisibilityProperty, value);
        }

        public int? MaxValue
        {
            get => _maxValue;

            set
            {
                if (value != null && Value != null && (InputType == TextBoxInputType.Double || InputType == TextBoxInputType.Integer || InputType == TextBoxInputType.Financial))
                {
                    _maxValue = value;

                    if (Convert.ToInt32(Value) > _maxValue)
                    {
                        Value = _maxValue;
                    }
                }
            }
        }

        public int? MinValue
        {
            get => _minValue;

            set
            {
                if (value != null && Value != null && (InputType == TextBoxInputType.Double || InputType == TextBoxInputType.Integer || InputType == TextBoxInputType.Financial))
                {
                    _minValue = value;

                    if (Convert.ToInt32(Value) < _minValue)
                    {
                        Value = _minValue;
                    }
                }
            }
        }

        public string IconName
        {
            get => (string)GetValue(IconNameProperty);
            set => SetValue(IconNameProperty, value);
        }

        public Brush ReadOnlyForeground
        {
            get => (Brush)GetValue(ReadOnlyForegroundProperty);
            set => SetValue(ReadOnlyForegroundProperty, value);
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

        public new bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

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

        public bool IsMouseLeftButtonDown
        {
            get => (bool)GetValue(IsMouseLeftButtonDownProperty);
            private set => SetValue(IsMouseLeftButtonDownPropertyKey, value);
        }

        public TextBoxInputType InputType
        {
            get => (TextBoxInputType)GetValue(InputTypeProperty);
            set => SetValue(InputTypeProperty, value);
        }
        
        public Visibility IconVisibility
        {
            get => (Visibility)GetValue(IconVisibilityProperty);
            set => SetValue(IconVisibilityProperty, value);
        }
        
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _currencySign = GetTemplateChild("CurrencySign") as TextBlock;

            _iconOverlay = GetTemplateChild("IconOverlay") as Rectangle;
            _iconBorder = GetTemplateChild("PART_IconBorder") as Border;
            if (_iconBorder != null)
            {
                _iconBorder.MouseEnter += IconBorder_MouseEnter;
                _iconBorder.MouseLeftButtonDown += IconBorder_MouseLeftButtonDown;
                _iconBorder.MouseLeftButtonUp += IconBorder_MouseLeftButtonUp;
                _iconBorder.MouseLeave += IconBorder_MouseLeave;
            }

            _iconBorderTransform = GetTemplateChild("IconBorderTransform") as TranslateTransform;

            OnIsReadOnlyChanged(IsReadOnly);
            OnInputTypeChanged(InputType);

            TextChanged -= OnTextChanged;
            TextChanged += OnTextChanged;

            if (InputType == TextBoxInputType.Financial)
            {
                FormatText();
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            HasText = Text.Length != 0;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            TimeSpan timeSpan = default(TimeSpan);
            if (_timeStamp != default(DateTime))
            {
                timeSpan = DateTime.Now - _timeStamp;
            }

            _timeStamp = DateTime.Now;

            if (timeSpan > default(TimeSpan) && timeSpan < TimeSpan.FromMilliseconds(50))
            {
                return;
            }

            if (HasText)
            {
                if (e.Key == Key.Space || e.Key == Key.Return)
                {
                    var words = Text.Split(' ');
                    if (words.Length > 1)
                    {
                        string lastWord = words[words.Length - 2];

                        if (lastWord != _lastReadWord)
                        {
                            _lastReadWord = lastWord;
                        }
                    }
                }
                else if (e.Key >= Key.A && e.Key <= Key.Z)
                {
                    _lastKey = e.Key.ToString();
                }
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    _lastKey = (e.Key - Key.NumPad0).ToString();
                }
                else if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    _lastKey = (e.Key - Key.D0).ToString();
                }
            }
            else
            {
                _lastReadWord = null;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (IsMouseOver && UpDownButtonVisibility == Visibility.Visible)
            {
                if (e.Delta > 0)
                {
                    IncrementValue();
                }
                else
                {
                    IncrementValue(false);
                }

                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            _lastKeyPressed = e.Key;

            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Up)
            {
                IncrementValue();
            }
            else if (e.Key == Key.Down)
            {
                IncrementValue(false);
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (InputType == TextBoxInputType.Financial)
            {
                SelectText();
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (InputType == TextBoxInputType.Financial || InputType == TextBoxInputType.PhoneNumber ||
                InputType == TextBoxInputType.Postcode || InputType == TextBoxInputType.Percentage)
            {
                FormatText();
            }

            base.OnLostFocus(e);
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox ctl)
            {
                ctl.OnIsReadOnlyChanged((bool)e.NewValue);
                ((System.Windows.Controls.TextBox)ctl).IsReadOnly = (bool)e.NewValue;
            }
        }

        private static void OnIconVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox ctl)
            {
                // ctl.OnInputTypeChanged((TextBoxInputType)e.NewValue);
            }
        }

        private static void OnInputTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox ctl)
            {
                ctl.OnInputTypeChanged((TextBoxInputType)e.NewValue);
            }
        }

        private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox ctl)
            {
            }
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox ctl)
            {
                if (e.NewValue != null)
                {
                    ctl.IconVisibility = Visibility.Visible;
                }
            }
        }
       
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox ctl)
            {
                ctl.OnValueChanged(e);
            }
        }

        private void IconBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_iconOverlay != null)
            {
                _iconOverlay.Opacity = 0.05;
            }         
        }

        private void IconBorder_MouseLeftButtonDown(object obj, MouseButtonEventArgs e)
        {
            IsMouseLeftButtonDown = true;
            _iconBorderTransform.X = 0.5;
            _iconBorderTransform.Y = 0.5;

            e.Handled = true;
        }

        private void IconBorder_MouseLeftButtonUp(object obj, MouseButtonEventArgs e)
        {
            if (IsMouseLeftButtonDown)
            {
                RaiseIconClickedEvent();

                IsMouseLeftButtonDown = false;
                _iconBorderTransform.X = 0;
                _iconBorderTransform.Y = 0;
            }
        }

        private void IconBorder_MouseLeave(object obj, MouseEventArgs e)
        {
            IsMouseLeftButtonDown = false;

            if (_iconOverlay != null)
            {
                _iconOverlay.Opacity = 0.2;
            }
        }

        private void RaiseIconClickedEvent()
        {
            var args = new RoutedEventArgs(IconClickedEvent);
            RaiseEvent(args);
        }

        private bool IsDigit(Key k)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                return false;
            }

            return (k >= Key.NumPad0 && k <= Key.NumPad9) || (k >= Key.D0 && k <= Key.D9);
        }

        private bool IsAlpha(Key k, bool includeSpace = true, bool includeHyphen = true)
        {
            if (includeSpace && k == Key.Space)
            {
                return true;
            }

            if (includeHyphen && k == Key.Subtract)
            {
                return true;
            }

            return k >= Key.A && k <= Key.Z;
        }

        private bool IsAlphaOrDigit(Key k, bool includeSpace = true, bool includeHyphen = true)
        {
            return IsAlpha(k, includeSpace, includeHyphen) || IsDigit(k);
        }

        private bool IsNavigationKey(Key k)
        {
            return k == Key.Back || k == Key.Tab || k == Key.Home || k == Key.End || k == Key.PageDown || k == Key.PageUp;
        }

        private bool IsDecimalPoint(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                return false;
            }

            return e.Key == Key.Decimal;
        }

        private void SelectText()
        {
            SelectionStart = 0;
            SelectionLength = (Text + string.Empty).Length;
        }

        private void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_enableUpdates && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
            {
                if (InputType == TextBoxInputType.Financial)
                {
                    decimal n = Convert.ToDecimal(e.NewValue);
                    Text = n.ToString("N2");
                }                
                else
                {
                    Text = e.NewValue.ToString();
                }
            }
            else if (_enableUpdates && e.NewValue == null)
            {
                Text = null;
            }
        }

        private bool AutoCompleteInput()
        {
            if (IsAlphaOrDigit(_lastKeyPressed))
            {
                string newText = Text.ToLower();

                if (AutoCompleteItemsSource != null && newText.Length > 1)
                {
                    var result = AutoCompleteItemsSource.Where(s => s.ToLower().IndexOf(newText) == 0);
                    if (result.Count() == 1)
                    {
                        Text = result.FirstOrDefault();

                        SelectionStart = newText.Length;
                        SelectionLength = Text.Length - newText.Length;

                        return true;
                    }
                }
            }

            return false;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;

            _enableUpdates = false;

            bool valueSet = false;
            string newText = tb.Text;

            // if (AutoCompleteInput())
            // {
            //     Value = tb.Text;
            //     enableUpdates = true;
            //     return;
            // } 
            int selStart = tb.SelectionStart;
            int originalLength = tb.Text.Length;
            if (originalLength > 0)
            {
                switch (InputType)
                {
                    case TextBoxInputType.Double:
                        {
                            newText = Regex.Replace(tb.Text, "[^0-9\\-.]", string.Empty);
                            if (double.TryParse(tb.Text, out double d))
                            {
                                if (!(Value is double) || (double)Value != d)
                                {
                                    Value = d;
                                }
                            }

                            valueSet = true;
                            break;
                        }

                    case TextBoxInputType.Financial:
                        {
                            newText = Regex.Replace(tb.Text, FinancialPattern, string.Empty);
                            if (decimal.TryParse(tb.Text, out decimal dec))
                            {
                                if (!(Value is decimal) || (decimal)Value != dec)
                                {
                                    Value = dec;
                                }
                            }

                            valueSet = true;
                            break;
                        }             
                    
                    case TextBoxInputType.Integer:
                        {
                            newText = Regex.Replace(tb.Text, "[^0-9\\-]", string.Empty);
                            if (int.TryParse(tb.Text, out int n))
                            {
                                if (!(Value is int) || (int)Value != n)
                                {
                                    Value = n;
                                }

                                if (MaxValue != null && n > MaxValue)
                                {
                                    Value = MaxValue;
                                    newText = MaxValue.ToString();
                                }

                                if (MinValue != null && n < MinValue)
                                {
                                    Value = MinValue;
                                    newText = MinValue.ToString();
                                }
                            }

                            valueSet = true;
                            break;
                        }

                    case TextBoxInputType.Percentage:
                        {
                            newText = Regex.Replace(tb.Text, PercentagePattern, string.Empty) + " %";
                            if (double.TryParse(tb.Text.Replace("%", string.Empty), out double p))
                            {
                                if (!(Value is double) || (double)Value != p)
                                {
                                    Value = p;
                                }
                            }

                            valueSet = true;
                            break;
                        }

                    case TextBoxInputType.Email:
                        {
                            newText = Regex.Replace(tb.Text, "[^a-zA-Z0-9\\-()_\\.@ ]", string.Empty).ToLower();
                            break;
                        }

                    case TextBoxInputType.AlphaNumeric:
                        {
                            newText = Regex.Replace(tb.Text, "[^a-zA-Z0-9\\-(),\\. ]", string.Empty);
                            break;
                        }

                    case TextBoxInputType.AlphaNumericAllCaps:
                        {
                            newText = Regex.Replace(tb.Text, "[^a-zA-Z0-9\\-(),\\. ]", string.Empty).ToUpper();
                            break;
                        }

                    case TextBoxInputType.Alpha:
                        {
                            newText = Regex.Replace(tb.Text, "[^a-zA-Z\\-(), ]", string.Empty);
                            break;
                        }

                    case TextBoxInputType.AllCaps:
                        {
                            newText = Regex.Replace(tb.Text, "[^a-zA-Z\\-(), ]", string.Empty).ToUpper();
                            break;
                        }

                    case TextBoxInputType.Postcode:
                        {
                            newText = Regex.Replace(tb.Text, "[^a-zA-Z0-9 ]", string.Empty).ToUpper();
                            break;
                        }

                    case TextBoxInputType.PhoneNumber:
                        {
                            newText = Regex.Replace(tb.Text, "[^0-9\\-()!# ]", string.Empty);
                            break;
                        }

                    case TextBoxInputType.CustomFormat:
                        {
                            if (InputPattern != null)
                            {
                                newText = Regex.Replace(tb.Text, InputPattern, string.Empty);
                            }

                            break;
                        }
                }

                if (newText != tb.Text)
                {
                    tb.Text = newText;
                }

                if (!valueSet && (Value == null || Value.ToString() != newText))
                {
                    Value = newText;
                }

                int newSelStart = selStart - (originalLength - tb.Text.Length);
                if (newSelStart < 0)
                {
                    newSelStart = 0;
                }

                tb.SelectionStart = newSelStart;
                tb.SelectionLength = 0;
            }
            else
            {
                Value = string.Empty;
            }

            _enableUpdates = true;
        }

        private void OnInputTypeChanged(TextBoxInputType inputType)
        {
            if (inputType == TextBoxInputType.Financial)
            {
                MaxLength = 16;
                if (_currencySign != null)
                {
                    _currencySign.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (_currencySign != null)
                {
                    _currencySign.Visibility = Visibility.Collapsed;
                }
            }

            if (inputType == TextBoxInputType.Percentage)
            {
                MaxLength = 8;
                UpDownButtonVisibility = !IsReadOnly ? Visibility.Visible : Visibility.Collapsed;
                FormatText();
            }          
        }

        private void OnIsReadOnlyChanged(bool isReadOnly)
        {
        }

        private void IncrementValue(bool inc = true)
        {
            if (!string.IsNullOrWhiteSpace(Text) && !IsReadOnly)
            {
                try
                {
                    switch (InputType)
                    {
                        case TextBoxInputType.Double:
                            {
                                double val = Convert.ToDouble(Text);
                                val += inc ? 1 : -1;
                                Text = val.ToString();
                            }

                            break;

                        case TextBoxInputType.Financial:
                            {
                                decimal val = Convert.ToDecimal(Text);
                                val += inc ? 1 : -1;
                                Text = val.ToString();
                            }

                            break;

                        case TextBoxInputType.Integer:
                            {
                                int val = Convert.ToInt32(Text);
                                val += inc ? 1 : -1;
                                if (MaxValue != null)
                                {
                                    val = Math.Min(val, MaxValue.Value);
                                }

                                if (MinValue != null)
                                {
                                    val = Math.Max(val, MinValue.Value);
                                }

                                Text = val.ToString();
                            }

                            break;

                        case TextBoxInputType.Percentage:
                            {
                                double val = Convert.ToDouble(Text.Replace("%", string.Empty));
                                val += inc ? 1 : -1;
                                if (val > 100)
                                {
                                    val = 100;
                                }

                                if (val < 0)
                                {
                                    val = 0;
                                }

                                Text = val.ToString();
                            }

                            break;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void FormatText()
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                string formattedText = Text;

                if (InputType == TextBoxInputType.Financial)
                {
                    string stringVal = Regex.Replace(Text + string.Empty, FinancialPattern, string.Empty);
                    if (decimal.TryParse(stringVal, out decimal dec))
                    {
                        formattedText = dec.ToString("N2");
                    }
                    else
                    {
                        formattedText = string.Empty;
                    }
                }               
                else if (InputType == TextBoxInputType.PhoneNumber)
                {
                }
                else if (InputType == TextBoxInputType.Postcode)
                {
                }
                else if (InputType == TextBoxInputType.Percentage)
                {
                    string newText = Regex.Replace(Text + string.Empty, PercentagePattern, string.Empty);
                    if (newText.Length > 0)
                    {
                        double val = Convert.ToDouble(newText);
                        if (val > 100)
                        {
                            val = 100;
                        }

                        if (val < 0)
                        {
                            val = 0;
                        }

                        formattedText = $"{val:N2} %";
                    }
                    else
                    {
                        formattedText = "0";
                    }
                }

                if (formattedText != Text)
                {
                    Text = formattedText;
                }
            }
        }

        private void SetupCommandBindings()
        {
            CommandBindings.Add(new CommandBinding(UpDownButtonUpCommand, UpDownButtonUpCommand_Executed));
            CommandBindings.Add(new CommandBinding(UpDownButtonDownCommand, UpDownButtonDownCommand_Executed));
        }

        private void UpDownButtonUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IncrementValue();
        }

        private void UpDownButtonDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IncrementValue(false);
        }
    }
}
