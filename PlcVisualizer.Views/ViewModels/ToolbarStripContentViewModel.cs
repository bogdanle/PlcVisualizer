using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using UI.Infrastructure;
using UI.Infrastructure.Interfaces;

namespace PlcVisualizer.ViewModels;

public class ToolbarStripContentViewModel : ViewModelCore<ToolbarStripContentViewModel>
{
    public ToolbarStripContentViewModel(
        IMessenger messenger,
        IMessageBoxService messageBox,
        IFileDialogService fileDialog,
        IErrorDialogService errorDialog,
        ILogger<ToolbarStripContentViewModel> logger)
        : base(messenger, messageBox, fileDialog, errorDialog, logger)
    {
    }
}
