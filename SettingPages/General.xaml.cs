using NekoDeskTop.Module;
using NekoDeskTop.Neko;
using NekoDeskTop.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                            Label = "Set the number of spectrum bars:\t",
                            Value = "1024",
                            OnValueChanged = (item, newValue) =>
                            {
                                if (int.TryParse(newValue, out int volume))
                                {
                                    var music = Application.Current.Resources["Music"] as Music;
                                    music.BarCount = volume;
                                }
                            }
                        },
                        new InputItem
                        {
                            Label = "Set spectrum thickness:\t",
                            Value = "2.5",
                            OnValueChanged = (item, newValue) =>
                            {
                                if (int.TryParse(newValue, out int volume))
                                {
                                    var music = Application.Current.Resources["Music"] as Music;
                                    music.BarThickness = volume;
                                }
                            }
                        },
                        new InputItem
                        {
                            Label = "Set the rounded corners of the spectrum:\t",
                            Value = "0",
                            OnValueChanged = (item, newValue) =>
                            {
                                if (int.TryParse(newValue, out int volume))
                                {
                                    var music = Application.Current.Resources["Music"] as Music;
                                    music.BarCornerRadius = volume;
                                }
                            }
                        },
                        new InputItem
                        {
                            Label = "Window.X:\t",
                            Value = "0",
                            OnValueChanged = (item, newValue) =>
                            {
                                if (int.TryParse(newValue, out int volume))
                                {
                                    var music = Application.Current.Resources["Music"] as Music;
                                    music.WindowStartupLocation = WindowStartupLocation.Manual;
                                    music.Left = volume;
                                }
                            }
                        },
                        new InputItem
                        {
                            Label = "Window.Y:\t",
                            Value = "0",
                            OnValueChanged = (item, newValue) =>
                            {
                                if (int.TryParse(newValue, out int volume))
                                {
                                    var music = Application.Current.Resources["Music"] as Music;
                                    music.WindowStartupLocation = WindowStartupLocation.Manual;
                                    music.Top = volume;
                                }
                            }
                        },
                        new InputItem
                        {
                            Label = "Window.Height:\t",
                            Value = "100",
                            OnValueChanged = (item, newValue) =>
                            {
                                if (int.TryParse(newValue, out int volume))
                                {
                                    var music = Application.Current.Resources["Music"] as Music;
                                    music.Height = volume;
                                }
                            }
                        },
                        new InputItem
                        {
                            Label = "Window.Width:\t",
                            Value = "100",
                            OnValueChanged = (item, newValue) =>
                            {
                                if (int.TryParse(newValue, out int volume))
                                {
                                    var music = Application.Current.Resources["Music"] as Music;
                                    music.Width = volume;
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
}