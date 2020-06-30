using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI.Controls
{
    public class HighlightingTextBlock : Control
    {
        public static readonly DependencyProperty HighlightedTextProperty = DependencyProperty.Register("HighlightedText", typeof(string), typeof(HighlightingTextBlock), new PropertyMetadata(OnHighlightedTextPropertyChanged));
        public static readonly DependencyProperty KeywordDelimiterCharProperty = DependencyProperty.Register("KeywordDelimiterCharProperty", typeof(char), typeof(HighlightingTextBlock), new PropertyMetadata(';'));
        public static readonly DependencyProperty HighlightFontBrushProperty = DependencyProperty.Register("HighlightFontBrush", typeof(Brush), typeof(HighlightingTextBlock), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnHighlightFontBrushPropertyChanged));
        public static readonly DependencyProperty SelectionBorderRadiusProperty = DependencyProperty.Register("SelectionBorderRadius", typeof(CornerRadius), typeof(HighlightingTextBlock), new PropertyMetadata(new CornerRadius(3), OnSelectionBorderRadiusPropertyChanged));
        public static readonly DependencyProperty HighlightFontWeightProperty = DependencyProperty.Register("HighlightFontWeight", typeof(FontWeight), typeof(HighlightingTextBlock), new PropertyMetadata(FontWeights.Bold, OnHighlightFontWeightPropertyChanged));
        public static readonly DependencyProperty SelectionBorderThicknessProperty = DependencyProperty.Register("SelectionBorderThickness", typeof(Thickness), typeof(HighlightingTextBlock), new PropertyMetadata(new Thickness(1), OnSelectionBorderThicknessPropertyChanged));
        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(HighlightingTextBlock), new PropertyMetadata(TextTrimming.None));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(HighlightingTextBlock), new PropertyMetadata(OnTextPropertyChanged));
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(HighlightingTextBlock), new PropertyMetadata(TextWrapping.NoWrap));
        public static readonly DependencyProperty SelectionBorderBrushProperty = DependencyProperty.Register("SelectionBorderBrush", typeof(Brush), typeof(HighlightingTextBlock), new PropertyMetadata(null, OnSelectionBorderBrushPropertyChanged));
        public static readonly DependencyProperty SelectionBackgroundBrushProperty = DependencyProperty.Register("SelectionBackgroundBrush", typeof(Brush), typeof(HighlightingTextBlock), new PropertyMetadata(null, OnSelectionBackgroundBrushPropertyChanged));
        private const double LeftOffset = 1;
        private const double RightOffset = -1;
        private const double HeightOffset = 1;
        private readonly List<Rectangle> _selectionRectangles = new List<Rectangle>();
        private Grid _rootElement;
        private TextBlock _textBlock;
        private List<Inline> _inlines;
        private FormattedText _formattedText;

        public HighlightingTextBlock()
        {
            HighlightingMode = Mode.HighlightBackground;
            MinHighlightedTextLength = 1;

            // DefaultStyleKey = typeof(HighlightingTextBlock);
            Style = (Style)Application.Current.Resources["HighlightingTextBlockStyle"];
        }

        public enum Mode
        {
            HighlightText,
            HighlightBackground
        }

        public string Text
        {
            get => GetValue(TextProperty) as string;
            set => SetValue(TextProperty, value);
        }

        public TextTrimming TextTrimming
        {
            get => (TextTrimming)GetValue(TextBlock.TextTrimmingProperty);
            set => SetValue(TextBlock.TextTrimmingProperty, value);
        }

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextBlock.TextWrappingProperty);
            set => SetValue(TextBlock.TextWrappingProperty, value);
        }

        public string HighlightedText
        {
            get => GetValue(HighlightedTextProperty) as string;
            set => SetValue(HighlightedTextProperty, value);
        }

        public char KeywordDelimiterChar
        {
            get => (char)GetValue(KeywordDelimiterCharProperty);
            set => SetValue(KeywordDelimiterCharProperty, value);
        }

        public Brush HighlightFontBrush
        {
            get => (Brush)GetValue(HighlightFontBrushProperty);
            set => SetValue(HighlightFontBrushProperty, value);
        }

        public Brush SelectionBackgroundBrush
        {
            get => GetValue(SelectionBackgroundBrushProperty) as Brush;
            set => SetValue(SelectionBackgroundBrushProperty, value);
        }

        public Brush SelectionBorderBrush
        {
            get => GetValue(SelectionBorderBrushProperty) as Brush;
            set => SetValue(SelectionBorderBrushProperty, value);
        }

        public FontWeight HighlightFontWeight
        {
            get => (FontWeight)GetValue(HighlightFontWeightProperty);
            set => SetValue(HighlightFontWeightProperty, value);
        }

        public Thickness SelectionBorderThickness
        {
            get => (Thickness)GetValue(SelectionBorderThicknessProperty);
            set => SetValue(SelectionBorderThicknessProperty, value);
        }

        public CornerRadius SelectionBorderRadius
        {
            get => (CornerRadius)GetValue(SelectionBorderRadiusProperty);
            set => SetValue(SelectionBorderRadiusProperty, value);
        }

        public Mode HighlightingMode { get; set; }

        public bool CaseSensitiveSearch { get; set; }

        public int MinHighlightedTextLength { get; set; }

        public string ColumnName { get; set; }
      
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootElement = GetTemplateChild("rootElement") as Grid;
            _textBlock = GetTemplateChild("textBlock") as TextBlock;

            OnTextPropertyChanged(Text);

            switch (HighlightingMode)
            {
                case Mode.HighlightBackground:
                    HighlightText();
                    break;

                case Mode.HighlightText:
                    ApplyHighlighting();
                    break;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (HighlightingMode == Mode.HighlightBackground)
            {
                HighlightText(true);
            }
        }

        protected virtual void HighlightText(bool updateMode = false)
        {
            if (_rootElement != null && HighlightedText != null && Text != null && _formattedText != null && ActualWidth > 0)
            {
                updateMode = (_rootElement.Children.Count > 1) && updateMode;

                if (updateMode == false)
                {
                    // Clear the rectangles
                    for (int i = _rootElement.Children.Count - 1; i >= 0; i--)
                    {
                        var child = _rootElement.Children[i];
                        if (child is Rectangle)
                        {
                            _rootElement.Children.Remove(child);
                        }

                        _selectionRectangles.Clear();
                    }
                }
                else
                {
                    // Reset all rectangles
                    foreach (var rect in _selectionRectangles)
                    {
                        rect.Width = 0; 
                        rect.Height = 0;
                    }
                }

                var parts = HighlightedText.Trim().Split(KeywordDelimiterChar);
                int j = 0;
                foreach (var part in parts)
                {
                    var text = part.Trim();

                    // Do the highlighting only for matched column
                    if (string.IsNullOrEmpty(ColumnName) == false && text.IndexOf('|') != -1)
                    {
                        int pos = text.IndexOf(ColumnName + '|', StringComparison.OrdinalIgnoreCase);
                        if (pos == 0)
                        {
                            text = text.Substring((ColumnName + '|').Length).TrimStart();
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (text.Length >= MinHighlightedTextLength)
                    {
                        var compare = CaseSensitiveSearch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                        for (int pos = 0; pos < Text.Length; pos++)
                        {
                            pos = Text.IndexOf(text, pos, compare);

                            if (pos != -1)
                            {
                                HighlightSubstring(pos, text.Length, j, updateMode);
                            }
                            else
                            {
                                HighlightSubstring(0, 0, j, updateMode);
                                break;
                            }

                            j++;
                        }
                    }
                }
            }
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HighlightingTextBlock obj)
            {
                string text = e.NewValue == null ? string.Empty : e.NewValue as string;        
                obj.OnTextPropertyChanged(text);
            }
        }

        private static void OnHighlightedTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HighlightingTextBlock obj)
            {
                switch (obj.HighlightingMode)
                {
                    case Mode.HighlightBackground:
                        obj.HighlightText();
                        break;

                    case Mode.HighlightText:
                        obj.ApplyHighlighting();
                        break;
                }
            }
        }

        private static void OnSelectionBackgroundBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnSelectionBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnSelectionBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnSelectionBorderRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnHighlightFontWeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HighlightingTextBlock;
            var value = (FontWeight)e.NewValue;
        }

        private static void OnHighlightFontBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HighlightingTextBlock;
            var value = (Brush)e.NewValue;
        }

        private void OnTextPropertyChanged(string text)
        {
            if (text == null)
            {
                return;
            }

            switch (HighlightingMode)
            {
                case Mode.HighlightBackground:
                    _formattedText = new FormattedText(
                                                text,
                                                CultureInfo.GetCultureInfo("en-us"),
                                                FlowDirection,
                                                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                                                FontSize,
                                                Foreground,
                                                null,
                                                TextOptions.GetTextFormattingMode(this));

                    if (_textBlock != null)
                    {
                        _textBlock.Text = text;
                    }

                    HighlightText();
                    break;

                case Mode.HighlightText:
                    if (_textBlock != null)
                    {
                        while (_textBlock.Inlines.Count > 0)
                        {
                            _textBlock.Inlines.Remove(_textBlock.Inlines.First());
                        }

                        var value = text;
                        _inlines = new List<Inline>();
                        for (int i = 0; i <= value.Length - 1; i++)
                        {
                            Inline run = new Run { Text = value[i].ToString() };
                            _textBlock.Inlines.Add(run);
                            _inlines.Add(run);
                        }

                        ApplyHighlighting();
                    }

                    break;
            }
        }
        
        private void AddRectangle(Rect bounds)
        {
            var rect = new Rectangle
            {
                Width = bounds.Width,
                Height = bounds.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = SelectionBackgroundBrush,
                Stroke = SelectionBorderBrush,
                StrokeThickness = 1,
                RadiusX = SelectionBorderRadius.TopLeft,
                RadiusY = SelectionBorderRadius.TopLeft,
                RenderTransform = new TranslateTransform(bounds.Left, bounds.Top)
            };

            _rootElement.Children.Insert(0, rect);
            _selectionRectangles.Add(rect);
        }

        private void UpdateRectangle(Rect bounds, int index)
        {
            if (index >= 0 && index < _selectionRectangles.Count)
            {
                var rect = _selectionRectangles[index];
                rect.Width = bounds.Width;
                rect.Height = bounds.Height;

                if (rect.RenderTransform is TranslateTransform transform)
                {
                    transform.X = bounds.Left;
                    transform.Y = bounds.Top;
                }
            }
        }

        private void HighlightSubstring(int start, int length, int index, bool updateMode)
        {
            _formattedText.MaxTextHeight = Math.Max(1, ActualHeight);
            _formattedText.MaxTextWidth = Math.Max(1, ActualWidth);
            _formattedText.Trimming = TextTrimming;

            var geometry = _formattedText.BuildHighlightGeometry(new Point(0, 0), start, length);
            if (geometry != null)
            {
                if (updateMode)
                {
                    UpdateRectangle(geometry.Bounds, index);
                }
                else
                {
                    AddRectangle(geometry.Bounds);    
                }
            }
        }

        private void ApplyHighlighting()
        {
            if (_inlines == null)
            {
                return;
            }

            string text = Text ?? string.Empty;
            string highlight = HighlightedText ?? string.Empty;
            const StringComparison compare = StringComparison.OrdinalIgnoreCase;

            int cur = 0;
            while (cur < text.Length)
            {
                int i = highlight.Length == 0 ? -1 : text.IndexOf(highlight, cur, compare);
                i = i < 0 ? text.Length : i;

                // Clear                
                while (cur < i && cur < text.Length)
                {
                    _inlines[cur].Foreground = Foreground;
                    _inlines[cur].FontWeight = FontWeight;
                    cur += 1;
                }

                // Highlight                
                int start = cur;
                while (cur < start + highlight.Length && cur < text.Length)
                {
                    _inlines[cur].Foreground = HighlightFontBrush;
                    _inlines[cur].FontWeight = HighlightFontWeight;
                    cur += 1;
                }
            }
        }
    }
}