using System.Windows.Controls;
using PlcVisualizer.ViewModels;
using UI.Infrastructure.Attributes;

namespace PlcVisualizer.Views
{
    [View(
        MenuLabel = "Application Log",
        ModuleName = "Admin",
        ToolbarStripContentType = typeof(ToolbarStripContent),
        ViewModelType = typeof(PlcViewModel))]
    public partial class PlcView
    {
        public PlcView()
        {
            InitializeComponent();

            //PostInitialize();            
        }       
    }
}