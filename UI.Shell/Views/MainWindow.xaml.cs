using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using UI.Shell.ViewModels;

namespace UI.Shell.Views;

public partial class MainWindow
{
    public static readonly RoutedCommand UndoCommand = new();
    public static readonly RoutedCommand HistoryCommand = new();
    public static readonly RoutedCommand ToggleViewCommand = new();

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
    }

    private void MainWindow_OnClosing(object sender, CancelEventArgs e)
    {
        dynamic vm = DataContext;
        vm.OnClosing();
    }

    private void OnUndo(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void OnHistory(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void OnToggleView(object sender, ExecutedRoutedEventArgs e)
    {
    }
}
