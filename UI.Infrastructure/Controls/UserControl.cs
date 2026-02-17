using System.ComponentModel;

namespace UI.Infrastructure.Controls;

public class UserControl : System.Windows.Controls.UserControl
{
    protected void PostInitialize()
    {
        dynamic vm = DataContext;
        if (vm != null)
        {
            vm.View = this;
        }
        else
        {
            DependencyPropertyDescriptor.FromProperty(DataContextProperty, typeof(UserControl)).AddValueChanged(this, (s, e) =>
            {
                dynamic viewModel = DataContext;
                viewModel.View = this;
            });
        }
    }
}