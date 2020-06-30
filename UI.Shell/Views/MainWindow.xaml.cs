using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace UI.Shell.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow
    {
        public static readonly RoutedCommand UndoCommand = new RoutedCommand();
        public static readonly RoutedCommand HistoryCommand = new RoutedCommand();
        public static readonly RoutedCommand ToggleViewCommand = new RoutedCommand();
        
        public MainWindow()
        {
            InitializeComponent();
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
}
