using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NekoDeskTop.Module
{
    public class ItemGroup : INotifyPropertyChanged
    {

        private bool? _isAllSelected;
        public bool? IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (_isAllSelected != value)
                {
                    _isAllSelected = value;
                    OnPropertyChanged(nameof(IsAllSelected));
                    if (value.HasValue)
                    {
                        foreach (var item in Items.OfType<SelectableItem>())
                        {
                            item.IsSelected = value.Value;
                        }
                    }
                }
            }
        }
        public string GroupName { get; set; } = "";
        public ObservableCollection<object> Items { get; set; } = new ObservableCollection<object>();
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class SelectableItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _name = string.Empty;
        private bool _isEnabled = true;
        private object? _tag;
        public int Id { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value && _isEnabled)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    OnSelectionChanged?.Invoke(this, value);

                    if (value)
                    {
                        OnSelected?.Invoke(this);
                    }
                    else
                    {
                        OnDeselected?.Invoke(this);
                    }
                }
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                    if (!value && _isSelected)
                    {
                        IsSelected = false;
                    }
                }
            }
        }

        public object? Tag
        {
            get => _tag;
            set
            {
                if (_tag != value)
                {
                    _tag = value;
                    OnPropertyChanged(nameof(Tag));
                }
            }
        }
        public Action<SelectableItem>? OnSelected { get; set; }
        public Action<SelectableItem>? OnDeselected { get; set; }

        public Action<SelectableItem, bool>? OnSelectionChanged { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Toggle()
        {
            IsSelected = !IsSelected;
        }
    }

    public enum InputType
    {
        Text,
        Number,
        Slider,
        PositionX,
        PositionY
    }
    public class InputItem : INotifyPropertyChanged
    {
        private string _value = "";
        private string _label = "";
        private bool _isEnabled = true;
        public InputType InputType { get; set; } = InputType.Text;
        public double MinValue { get; set; } = 0;
        public double MaxValue { get; set; } = 100;
        public double Step { get; set; } = 1;

        public string Label
        {
            get => _label;
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged(nameof(Label));
                }
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                    OnValueChanged?.Invoke(this, value);
                }
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        public Action<InputItem, string>? OnValueChanged { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
