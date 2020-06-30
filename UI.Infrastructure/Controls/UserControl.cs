using System;
using System.ComponentModel;
using Prism.Events;

namespace UI.Infrastructure.Controls
{
    public class UserControl : System.Windows.Controls.UserControl
    {
        protected IEventAggregator EventAggregator { get; set; }

        protected void PostInitialize()
        {
            try
            {
                //var container = DiContainer.GetInstance();
                //EventAggregator = container.Resolve<IEventAggregator>();
            }
            catch (Exception)
            {
            }

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
}
