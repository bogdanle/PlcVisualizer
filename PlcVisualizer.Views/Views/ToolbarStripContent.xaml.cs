using PlcVisualizer.ViewModels;
using UI.Infrastructure;

namespace PlcVisualizer.Views;

public partial class ToolbarStripContent
{
    public ToolbarStripContent()
    {
        InitializeComponent();

        DataContext = (PlcViewModel)AppServices.Provider.GetService(typeof(PlcViewModel));
    }
}
