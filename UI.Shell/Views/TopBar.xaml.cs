using UI.Infrastructure;
using UI.Shell.ViewModels;

namespace UI.Shell.Views;

public partial class TopBar
{
    public TopBar()
    {
        InitializeComponent();
        DataContext = (TopBarViewModel)AppServices.Provider.GetService(typeof(TopBarViewModel));
    }
}