using UI.Infrastructure;
using UI.Infrastructure.Events;
using Unity;

namespace PlcVisualizer.ViewModels
{
    public class ToolbarStripContentViewModel : ViewModelCore<ToolbarStripContentViewModel>
    {
        public ToolbarStripContentViewModel(IUnityContainer container) 
            : base(container)
        {
        }

        protected override void OnViewChanged()
        {
            EventAggregator.GetEvent<ConnectToViewModelEvent<PlcViewModel>>().Publish(View);
        }
    }
}
