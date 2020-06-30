using System;
using Prism.Mvvm;

namespace UI.Infrastructure.Models
{
    public class FilterDefinitionCore<T> : BindableBase
    {
        private bool _isSelected;
        private string _name;
        private int _count;

        public FilterDefinitionCore(string name, Func<T, bool> predicate = null, bool isSelected = false)
        {
            Name = name;
            Predicate = predicate;
            IsSelected = isSelected;
        }

        public event Action<FilterDefinitionCore<T>> FilterChanged;
     
        public string Name
        {
            get => _name;

            set
            {
                SetProperty(ref _name, value);
                RaisePropertyChanged(nameof(DisplayName));
            }
        }

        public Func<T, bool> Predicate { get; set; }

        public virtual string DisplayName => $"{Name} ({Count})";

        public int Type { get; set; }

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
    }
}