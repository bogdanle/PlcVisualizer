using System;
using System.Collections.ObjectModel;
using UI.Shell.Controls;

namespace UI.Shell.Interfaces
{
    public interface IMenuBuilder
    {
        event EventHandler MenuItemClicked;

        ObservableCollection<SidebarMenuItem> MenuItems { get; }

        void BuildMenu();
    }
}