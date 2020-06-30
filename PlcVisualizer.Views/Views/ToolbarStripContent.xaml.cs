namespace PlcVisualizer.Views
{
    public partial class ToolbarStripContent
    {
        public ToolbarStripContent()
        {
            InitializeComponent();

            Loaded += ToolbarStripContent_Loaded;
        }

        private void ToolbarStripContent_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            dynamic vm = DataContext;
            vm.View = this;
        }
    }
}
