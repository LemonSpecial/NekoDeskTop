using NekoDeskTop.Neko;
using NekoDeskTop.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace NekoDeskTop.SettingPages
{
    public partial class General : Page
    {
        public ObservableCollection<ItemGroup> ItemGroups { get; set; } = new ObservableCollection<ItemGroup>();

        public General()
        {
            InitializeComponent();
            InitListData();
        }

        private void InitListData()
        {
            ItemGroups = new ObservableCollection<ItemGroup>
    {
        new ItemGroup
        {
            GroupName = "Audio Visualization",
            Items = new ObservableCollection<object>
            {
                new SelectableItem
                {
                    Id = 1,
                    Name = "Whether to display components",
                    OnSelected = (item) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            var topz_index = Application.Current.Resources["TopZ_index"] as TopZ_index;
                            var setting = Application.Current.Resources["Setting"] as Setting;
                            if (music != null && setting != null)
                            {
                                music.Show();
                                topz_index.SetWindowAbove(music,setting);
                            }
                        });
                    },
                    OnDeselected = (item) =>
                    {
                        var music = Application.Current.Resources["Music"] as Music;
                        if(music != null)
                        {
                            music.Hide();
                        }
                    }
                },
                new InputItem
                {
                    Label = "Set the number of spectrum bars",
                    Value = "1024",
                    OnValueChanged = (item, newValue) =>
                    {
                        if (int.TryParse(newValue, out int volume))
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            music.BarCount = volume;
                        }
                    }
                }
            }
        }
    };

            foreach (var group in ItemGroups)
            {
                UpdateGroupSelectionState(group);

                group.Items.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is SelectableItem selectableItem)
                            {
                                selectableItem.PropertyChanged += Item_PropertyChanged;
                            }
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is SelectableItem selectableItem)
                            {
                                selectableItem.PropertyChanged -= Item_PropertyChanged;
                            }
                        }
                    }

                    UpdateGroupSelectionState(group);
                };

                foreach (var item in group.Items)
                {
                    if (item is SelectableItem selectableItem)
                    {
                        selectableItem.PropertyChanged += Item_PropertyChanged;
                    }
                }
            }

            this.DataContext = this;
        }

        private void UpdateGroupSelectionState(ItemGroup group)
        {
            var selectableItems = group.Items.OfType<SelectableItem>().ToList();

            if (selectableItems.Count == 0) return;

            if (selectableItems.All(i => i.IsSelected))
                group.IsAllSelected = true;
            else if (selectableItems.All(i => !i.IsSelected))
                group.IsAllSelected = false;
            else
                group.IsAllSelected = null;
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectableItem.IsSelected) && sender is SelectableItem item)
            {
                var group = ItemGroups.FirstOrDefault(g => g.Items.Contains(item));
                if (group != null)
                {
                    UpdateGroupSelectionState(group);
                }

                var allSelected = ItemGroups
                    .SelectMany(g => g.Items.OfType<SelectableItem>())
                    .Where(i => i.IsSelected)
                    .ToList();
            }
        }
    }

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
    public class InputItem : INotifyPropertyChanged
    {
        private string _value = "";
        private string _label = "";
        private bool _isEnabled = true;

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