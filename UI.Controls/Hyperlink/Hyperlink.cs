using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UI.Controls
{
    public class Hyperlink : Control
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(Hyperlink), new PropertyMetadata(null));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Hyperlink), new PropertyMetadata(null, OnTextChanged));        
        public static readonly DependencyProperty HoverLinkForegroundProperty = DependencyProperty.Register("HoverLinkForeground", typeof(Brush), typeof(Hyperlink), new PropertyMetadata(Brushes.LightSteelBlue));
        public static readonly DependencyProperty PressedLinkForegroundProperty = DependencyProperty.Register("PressedLinkForeground", typeof(Brush), typeof(Hyperlink), new PropertyMetadata(Brushes.Orange));
        public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.Register("NavigateUri", typeof(Uri), typeof(Hyperlink), new PropertyMetadata(null));
        private System.Windows.Documents.Hyperlink _hyperlink;
        private TextBlock _textBlock;
        private bool _flag;

        static Hyperlink()
        {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(Hyperlink), new FrameworkPropertyMetadata(typeof(Hyperlink)));
        }

        public Hyperlink()
        {
            Style = (Style)Application.Current.Resources["MyHyperlinkStyle"];
        }

        public static event Action<int, string> HyperlinkClicked;

        public Uri NavigateUri
        {
            get => (Uri)GetValue(NavigateUriProperty);
            set => SetValue(NavigateUriProperty, value);
        }

        public Brush HoverLinkForeground
        {
            get => (Brush)GetValue(HoverLinkForegroundProperty);
            set => SetValue(HoverLinkForegroundProperty, value);
        }

        public Brush PressedLinkForeground
        {
            get => (Brush)GetValue(PressedLinkForegroundProperty);
            set => SetValue(PressedLinkForegroundProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _hyperlink = new System.Windows.Documents.Hyperlink
            {
                Style = (Style)Application.Current.Resources["InlineHyperlinkStyle"]
            };

            _textBlock = new TextBlock();
            _hyperlink.Inlines.Add(_textBlock);
            _hyperlink.Click += Hyperlink_Click;

            var parent = GetTemplateChild("textBlock") as TextBlock;
            parent?.Inlines.Add(_hyperlink);

            OnTextChanged(Text);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            _hyperlink.Focus();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _flag = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_flag)
            {
                Hyperlink_Click(this, e);
            }

            _flag = false;
        }

        private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var ctl = (Hyperlink)obj;
            if (ctl._hyperlink != null)
            {
                ctl.OnTextChanged(args.NewValue as string);
            }
        }

        private void OnTextChanged(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var parts = text.Split(';');
                if (parts.Length == 2 && parts[1].Length > 0)
                {
                    _textBlock.Text = parts[0];
                    _hyperlink.NavigateUri = Uri.TryCreate(parts[1], UriKind.Absolute, out var uriResult) ? uriResult : null;
                }
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (Command != null && Command.CanExecute(null))
            {
                Command.Execute(null);
                return;
            }

            int? pos = Text?.IndexOf("^");
            if (pos != null && pos != -1)
            {
                var parts = Text.Substring(pos.Value + 1).Split(':');

                HyperlinkClicked?.Invoke(Convert.ToInt32(parts[0]), parts[1]);
                return;
            }

            var link = (System.Windows.Documents.Hyperlink)e.Source;
            if (link.NavigateUri != null)
            {
                try
                {
                    Process.Start(link.NavigateUri.ToString());
                }
                catch (Exception)
                {
                    MessageBox.Show($"Error opening {link.NavigateUri.ToString()}", "Invalid Link");
                }

                e.Handled = true;
            }
        }      
    }
}