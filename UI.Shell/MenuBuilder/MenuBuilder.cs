using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UI.Infrastructure.Attributes;
using UI.Shell.Controls;
using UI.Shell.Interfaces;

namespace UI.Shell.MenuBuilder;

public class MenuBuilder : IMenuBuilder
{
    private readonly IMenuBuilderCatalog _catalog;
    private List<SidebarMenuItem> _allMenuItems;

    public MenuBuilder(IMenuBuilderCatalog catalog)
    {
        _catalog = catalog;
    }

    public event EventHandler MenuItemClicked;

    public ObservableCollection<SidebarMenuItem> MenuItems { get; } = new ObservableCollection<SidebarMenuItem>();
        
    public void BuildMenu()
    {
        _allMenuItems = new List<SidebarMenuItem>();

        var viewTypes = _catalog.DiscoverViewTypes();
        foreach (var viewType in viewTypes)
        {
            var attr = (ViewAttribute)Attribute.GetCustomAttribute(viewType, typeof(ViewAttribute));
            if (attr != null)
            {
                var menuItem = new SidebarMenuSubitem()
                {
                    ViewName = viewType.FullName.Split().Last(),
                    ViewType = viewType,
                    ToolbarStripContentType = attr.ToolbarStripContentType,
                    ViewModelType = attr.ViewModelType,
                    ModuleName = attr.ModuleName,
                    Label = attr.MenuLabel
                };

                _allMenuItems.Add(menuItem);
                menuItem.ItemClicked += MenuItem_ItemClicked;
            }
        }
    }

    private void MenuItem_ItemClicked(object sender, EventArgs e)
    {
        MenuItemClicked?.Invoke(sender, e);
    }
}