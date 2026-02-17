using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PlcVisualizer.Models;

public class TagInfo : ObservableObject
{
    public string Name { get; set; }

    public string Description { get; set; }

    public object Value
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string DataType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string Quality
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTime? TimeStamp
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string Group { get; set; }

    public bool IsChecked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public object TagTemplate { get; set; }

    public object Tag
    {
        get;

        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(Quality));
                OnPropertyChanged(nameof(TimeStamp));
            }
        }
    }
}
