using System;
using System.ComponentModel;
using Prism.Mvvm;
using UI.Core.Extensions;

namespace UI.Infrastructure.Models
{
    public class FilterDefinition : BindableBase
    {
        private bool _isSelected;
        private string _name;
        private int _count;
        private string _filterString;

        public FilterDefinition(FilterType type = FilterType.Custom)
        {
            Type = type;
            Name = type.GetDescription();
        }

        public event Action<FilterDefinition> FilterChanged;

        public enum FilterType
        {
            None,
            Override,
            Warning,
            Error,            
            [Description("All Rows")]
            All,
            Custom
        }

        public FilterType Type { get; set; }

        public string Name
        {
            get => _name;

            set
            {
                SetProperty(ref _name, value);
                RaisePropertyChanged(nameof(DisplayName));
            }
        }

        public string FilterString
        {
            get => _filterString;
            set => SetProperty(ref _filterString, value);
        }

        public string DisplayName => Type == FilterType.Custom
            ? Name
            : (Type == FilterType.All ? $"{Name} ({Count})" : $"{Name}s ({Count})");

        public int Count
        {
            get => _count;

            set
            {
                SetProperty(ref _count, value);
                RaisePropertyChanged(nameof(DisplayName));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                SetProperty(ref _isSelected, value);
                FilterChanged?.Invoke(this);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}