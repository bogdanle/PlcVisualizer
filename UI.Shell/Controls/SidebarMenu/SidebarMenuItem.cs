using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace UI.Shell.Controls;

public abstract class SidebarMenuItem : INotifyPropertyChanged
{        
    private bool _isSelected;        
    private string _viewTitle;

    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler ItemClicked;

    public Type ViewType { get; set; }

    public Type ToolbarStripContentType { get; set; }

    public UserControl View { get; set; }

    public string Label { get; set; }

    public string ModuleName { get; set; }

    public string ViewTitle
    {
        get => _viewTitle ?? Label;
        set => _viewTitle = value;
    }
       
    public bool IsSelected
    {
        get => _isSelected;

        set
        {
            if (value != _isSelected)
            {
                _isSelected = value;
                OnPropertyChanged();

                if (value)
                {
                    ItemClicked?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    public int ModuleId { get; set; }

    public IList<SidebarMenuSubitem> Children { get; set; }

    public bool HasChildren => Children?.Count > 0;

    public string ViewName { get; set; }

    public Type ViewModelType { get; set; }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }        
}