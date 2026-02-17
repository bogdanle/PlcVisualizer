using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace UI.Controls;

/// <summary>
/// Interaction logic for ErrorDialog.xaml.
/// </summary>
public partial class ErrorDialog
{
    private readonly DoubleAnimation _panelExpandAnimation = new DoubleAnimation(0, 180, new Duration(TimeSpan.FromMilliseconds(200)))
    {
        EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.2 }
    };

    private readonly DoubleAnimation _panelCollapseAnimation = new DoubleAnimation(180, 0, new Duration(TimeSpan.FromMilliseconds(200)))
    {
        EasingFunction = new BackEase { EasingMode = EasingMode.EaseIn, Amplitude = 0.3 }
    };

    private ErrorDialog(Exception ex)
    {
        Exception = ex;

        InitializeComponent();

        ExceptionToRichText(ex);            
    }

    private Exception Exception { get; set; }

    public static void Show(Exception ex)
    {
        var dlg = new ErrorDialog(ex);
        dlg.Loaded += Dlg_Loaded;
        dlg.ShowDialog();
    }

    private static void Dlg_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void ExceptionToRichText(Exception ex)
    {
        var paragraph = new Paragraph();

        try
        {
            paragraph.Inlines.Add(new Bold(new Run("Exception details:")));

            paragraph.Inlines.Add(new Run(Environment.NewLine));
            paragraph.Inlines.Add(new Run(Environment.NewLine));
            paragraph.Inlines.Add(new Bold(new Run("Message:  ")));
            paragraph.Inlines.Add(new Run(Environment.NewLine));
            paragraph.Inlines.Add(new Run(ex.Message));

            paragraph.Inlines.Add(new Run(Environment.NewLine));
            paragraph.Inlines.Add(new Run(Environment.NewLine));
            paragraph.Inlines.Add(new Bold(new Run("Source:  ")));
            paragraph.Inlines.Add(new Run(Environment.NewLine));
            paragraph.Inlines.Add(new Run(ex.Source));

            paragraph.Inlines.Add(new Run(Environment.NewLine));
            paragraph.Inlines.Add(new Run(Environment.NewLine));

            if (ex.TargetSite != null)
            {
                paragraph.Inlines.Add(new Bold(new Run("Target site:  ")));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(ex.TargetSite.ToString()));
            }

            if (ex.StackTrace != null)
            {
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Bold(new Run("Stack trace:")));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(ex.StackTrace));
            }

            if (ex.InnerException != null)
            {
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Bold(new Run("Inner exception details:")));

                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Bold(new Run("Message:")));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(ex.InnerException.Message));

                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Bold(new Run("Source:")));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(ex.InnerException.Source));

                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Bold(new Run("Target site:")));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(ex.InnerException?.TargetSite?.ToString())); // Gave Null Exception

                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Bold(new Run("Stack trace:")));
                paragraph.Inlines.Add(new Run(Environment.NewLine));
                paragraph.Inlines.Add(new Run(ex.InnerException.StackTrace));
            }
        }
        catch
        {                                
        }
        finally
        {
            var flowDocument = new FlowDocument();
            flowDocument.Blocks.Add(paragraph);

            richTextBox.Document = flowDocument;
        }
    }
        
    private void ContinueButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void QuitButton_OnClick(object sender, RoutedEventArgs e)
    {
        Process.GetCurrentProcess().Kill();
    }

    private void ExpandButton_OnClick(object sender, RoutedEventArgs e)
    {
        detailsPanel.Visibility = Visibility.Visible;
        detailsPanel.BeginAnimation(HeightProperty, _panelExpandAnimation);
        collapseButton.Visibility = Visibility.Visible;
        expandButton.Visibility = Visibility.Hidden;
    }

    private void CollapseButton_OnClick(object sender, RoutedEventArgs e)
    {
        detailsPanel.BeginAnimation(HeightProperty, _panelCollapseAnimation);
        collapseButton.Visibility = Visibility.Hidden;
        expandButton.Visibility = Visibility.Visible;
    }
}