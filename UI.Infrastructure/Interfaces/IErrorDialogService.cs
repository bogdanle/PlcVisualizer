using System;

namespace UI.Infrastructure.Interfaces;

public interface IErrorDialogService
{
    void Show(Exception ex);
}