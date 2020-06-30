using System;
using Logix;
using Prism.Mvvm;

namespace PlcVisualizer.Models
{
    public class TagInfo : BindableBase
    {
        private string _dataType;
        private Tag _tag;
        private string _quality;
        private DateTime? _timeStamp;
        private object _value;
        private bool _isChecked;

        public string Name { get; set; }

        public string Description { get; set; }

        public object Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        
        public string DataType
        {
            get => _dataType ?? TagTemplate?.TypeName;
            set => SetProperty(ref _dataType, value);
        }

        public string Quality
        {
            get => _quality;
            set => SetProperty(ref _quality, value);
        }

        public DateTime? TimeStamp
        {
            get => _timeStamp;
            set => SetProperty(ref _timeStamp, value);
        }

        public string Group { get; set; }
        
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        public TagTemplate TagTemplate { get; set; }
       
        public Tag Tag
        {
            get => _tag;

            set
            {
                if (SetProperty(ref _tag, value))
                {
                    RaisePropertyChanged(nameof(Value));
                    RaisePropertyChanged(nameof(Quality));
                    RaisePropertyChanged(nameof(TimeStamp));
                }
            }
        }
    }
}
