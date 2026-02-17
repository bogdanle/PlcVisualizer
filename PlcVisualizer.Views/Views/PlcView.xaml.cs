using PlcVisualizer.ViewModels;
using UI.Infrastructure;
using UI.Infrastructure.Attributes;

namespace PlcVisualizer.Views;

[View(
    MenuLabel = "PLC View",
    ModuleName = "Views",
    ToolbarStripContentType = typeof(ToolbarStripContent),
    ViewModelType = typeof(PlcViewModel))]
public partial class PlcView
{
    public PlcView()
    {
        InitializeComponent();
        DataContext = (PlcViewModel)AppServices.Provider.GetService(typeof(PlcViewModel));
    }
}
