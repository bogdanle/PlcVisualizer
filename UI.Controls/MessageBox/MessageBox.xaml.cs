using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Automation;

namespace UI.Controls;

public partial class MessageBox
{
    private MessageBoxButton _button = MessageBoxButton.OK;
    private MessageBoxResult _defaultResult = MessageBoxResult.None;

    internal MessageBox()
    {
        InitializeComponent();
    }

    protected MessageBoxResult MessageBoxResult { get; set;  } = MessageBoxResult.None;

    public static MessageBoxResult Show(string messageText)
    {
        return ShowInternal(null, messageText, string.Empty, MessageBoxButton.OK);
    }

    public static MessageBoxResult Show(Window owner, string messageText)
    {
        return ShowInternal(owner, messageText, string.Empty, MessageBoxButton.OK);
    }

    public static MessageBoxResult Show(string messageText, string caption)
    {
        return ShowInternal(null, messageText, caption, MessageBoxButton.OK);
    }

    public static MessageBoxResult Show(Window owner, string messageText, string caption)
    {
        return ShowInternal(owner, messageText, caption, MessageBoxButton.OK);
    }

    public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button)
    {
        return ShowInternal(null, messageText, caption, button);
    }

    public static MessageBoxResult Show(Window owner, string messageText, string caption, MessageBoxButton button)
    {
        return ShowInternal(owner, messageText, caption, button);
    }

    public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
        return ShowInternal(null, messageText, caption, button, icon);
    }

    public static MessageBoxResult Show(Window owner, string messageText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
        return ShowInternal(owner, messageText, caption, button, icon);
    }

    public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
    {
        return ShowInternal(null, messageText, caption, button, icon, defaultResult);
    }

    public static MessageBoxResult Show(Window owner, string messageText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
    {
        return ShowInternal(owner, messageText, caption, button, icon, defaultResult);
    }

    private static Window ComputeOwnerWindow()
    {
        Window owner = null;
        if (Application.Current != null)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w.IsActive)
                {
                    owner = w;
                    break;
                }
            }
        }

        return owner;
    }

    private static MessageBoxResult ShowInternal(
        Window owner, 
        string messageText, 
        string caption, 
        MessageBoxButton button,
        MessageBoxImage icon = MessageBoxImage.None, 
        MessageBoxResult defaultResult = MessageBoxResult.None)
    {
        var mb = new MessageBox
        {
            Title = caption,
            MessageText = { Text = messageText }
        };

        switch (icon)
        {
            case MessageBoxImage.Information:
                mb.InfoIcon.Visibility = Visibility.Visible;
                break;

            case MessageBoxImage.Question:
                mb.QuestionIcon.Visibility = Visibility.Visible;
                break;

            case MessageBoxImage.Error:
                mb.ErrorIcon.Visibility = Visibility.Visible;
                break;

            case MessageBoxImage.Warning:
                mb.WarningIcon.Visibility = Visibility.Visible;
                break;
        }

        switch (button)
        {
            case MessageBoxButton.OK:
                mb.OkButton.Visibility = Visibility.Visible;
                mb.OkButton.Focus();
                break;

            case MessageBoxButton.OKCancel:
                mb.OkButton.Visibility = Visibility.Visible;
                mb.CancelButton.Visibility = Visibility.Visible;
                mb.OkButton.Focus();
                break;

            case MessageBoxButton.YesNo:
                mb.YesButton.Visibility = Visibility.Visible;
                mb.NoButton.Visibility = Visibility.Visible;
                mb.YesButton.Focus();
                break;

            case MessageBoxButton.YesNoCancel:
                mb.YesButton.Visibility = Visibility.Visible;
                mb.NoButton.Visibility = Visibility.Visible;
                mb.CancelButton.Visibility = Visibility.Visible;
                mb.YesButton.Focus();
                break;
        }

        var sb = new StringBuilder();

        sb.AppendFormat("Message box {0}, ", mb.Title);
        sb.AppendFormat("Message {0}", mb.MessageText.Text);

        AutomationProperties.SetName(mb, sb.ToString());

        var wnd = mb.Owner as Window;
        if (wnd != null)
        {
            wnd.IsGrayedOut = true;
        }

        // Disable the 'Close' button if Cancel is not an option
        if (mb.CancelButton.Visibility != Visibility.Visible)
        {
            mb.CanClose = false;
        }

        mb.ShowDialog();
        if (wnd != null)
        {
            wnd.IsGrayedOut = false;
        }

        return mb.MessageBoxResult;
    }

    private void SetDefaultResult()
    {
        var defaultButton = GetDefaultButtonFromDefaultResult();
        if (defaultButton != null)
        {
            defaultButton.IsDefault = true;
            defaultButton.Focus();
        }
    }

    private System.Windows.Controls.Button GetDefaultButtonFromDefaultResult()
    {
        System.Windows.Controls.Button defaultButton = null;
        switch (_defaultResult)
        {
            case MessageBoxResult.Cancel:
                defaultButton = CancelButton;
                break;

            case MessageBoxResult.No:
                defaultButton = NoButton;
                break;

            case MessageBoxResult.OK:
                defaultButton = OkButton;
                break;

            case MessageBoxResult.Yes:
                defaultButton = YesButton;
                break;

            case MessageBoxResult.None:
                defaultButton = GetDefaultButton();
                break;
        }

        return defaultButton;
    }

    private System.Windows.Controls.Button GetDefaultButton()
    {
        System.Windows.Controls.Button defaultButton = null;
        switch (_button)
        {
            case MessageBoxButton.OK:
            case MessageBoxButton.OKCancel:
                defaultButton = OkButton;
                break;

            case MessageBoxButton.YesNo:
            case MessageBoxButton.YesNoCancel:
                defaultButton = YesButton;
                break;
        }

        return defaultButton;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is System.Windows.Controls.Button button)
        {
            switch (button.Name)
            {
                case "NoButton":
                    MessageBoxResult = MessageBoxResult.No;
                    break;

                case "YesButton":
                    MessageBoxResult = MessageBoxResult.Yes;
                    break;

                case "CancelButton":
                    MessageBoxResult = MessageBoxResult.Cancel;
                    break;

                case "OkButton":
                    MessageBoxResult = MessageBoxResult.OK;
                    break;
            }

            e.Handled = true;
            Close();
        }
    }

    private void Window_OnClosing(object sender, CancelEventArgs e)
    {
        e.Cancel = !CanClose && MessageBoxResult == MessageBoxResult.None;
    }
}