using System;
using UI.Controls;
using UI.Infrastructure.Interfaces;

namespace UI.Infrastructure.Services;

public class ErrorDialogService : IErrorDialogService
{
    public void Show(Exception ex)
    {
        ErrorDialog.Show(ex);
    }        
}
