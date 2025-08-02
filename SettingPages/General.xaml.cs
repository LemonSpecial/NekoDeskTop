using NekoDeskTop.Module;
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
            FileSystem fileSystem = new FileSystem();

            string panelDisplay = fileSystem.Read("Audio Visualization", "PanelDisplay");
            string barCount = fileSystem.Read("Audio Visualization", "BarCount");
            string barThickness = fileSystem.Read("Audio Visualization", "BarThickness");
            string barCornerRadius = fileSystem.Read("Audio Visualization", "BarCornerRadius");
            string panelLocationX = fileSystem.Read("Audio Visualization", "PanelLocation_X");
            string panelLocationY = fileSystem.Read("Audio Visualization", "PanelLocation_Y");
            string panelWidth = fileSystem.Read("Audio Visualization", "PanelWidth");
            string panelHeight = fileSystem.Read("Audio Visualization", "PanelHeight");

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
                    IsSelected = panelDisplay.Equals("true", StringComparison.OrdinalIgnoreCase),
                    OnSelected = (item) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            var topz_index = Application.Current.Resources["TopZ_index"] as TopZ_index;
                            var setting = Application.Current.Resources["Setting"] as Setting;
                            if (music != null && setting != null)
                            {
                                fileSystem.Write("Audio Visualization", "PanelDisplay", "true");
                                music.Show();
                                topz_index.SetWindowAbove(music, setting);
                            }
                        });
                    },
                    OnDeselected = (item) =>
                    {
                        var music = Application.Current.Resources["Music"] as Music;
                        if (music != null)
                        {
                            fileSystem.Write("Audio Visualization", "PanelDisplay", "false");
                            music.Hide();
                        }
                    }
                },
                new InputItem
                {
                    Label = "Set the number of spectrum bars:\t",
                    Value = !string.IsNullOrEmpty(barCount) ? barCount : "1024",
                    OnValueChanged = (item, newValue) =>
                    {
                        if (int.TryParse(newValue, out int volume))
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            fileSystem.Write("Audio Visualization", "BarCount", volume.ToString());
                            music.BarCount = volume;
                        }
                    }
                },
                new InputItem
                {
                    Label = "Set spectrum thickness:\t",
                    Value = !string.IsNullOrEmpty(barThickness) ? barThickness : "2.5",
                    OnValueChanged = (item, newValue) =>
                    {
                        if (double.TryParse(newValue, out double thickness))
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            fileSystem.Write("Audio Visualization", "BarThickness", thickness.ToString());
                            music.BarThickness = thickness;
                        }
                    }
                },
                new InputItem
                {
                    Label = "Set the rounded corners of the spectrum:\t",
                    Value = !string.IsNullOrEmpty(barCornerRadius) ? barCornerRadius : "0",
                    OnValueChanged = (item, newValue) =>
                    {
                        if (double.TryParse(newValue, out double radius))
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            fileSystem.Write("Audio Visualization", "BarCornerRadius", radius.ToString());
                            music.BarCornerRadius = radius;
                        }
                    }
                },
                new InputItem
                {
                    Label = "Window.X:\t",
                    Value = !string.IsNullOrEmpty(panelLocationX) ? panelLocationX : "0",
                    OnValueChanged = (item, newValue) =>
                    {
                        if (double.TryParse(newValue, out double x))
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            fileSystem.Write("Audio Visualization", "PanelLocation_X", x.ToString());
                            music.WindowStartupLocation = WindowStartupLocation.Manual;
                            music.Left = x;
                        }
                    }
                },
                new InputItem
                {
                    Label = "Window.Y:\t",
                    Value = !string.IsNullOrEmpty(panelLocationY) ? panelLocationY : "0",
                    OnValueChanged = (item, newValue) =>
                    {
                        if (double.TryParse(newValue, out double y))
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            fileSystem.Write("Audio Visualization", "PanelLocation_Y", y.ToString());
                            music.WindowStartupLocation = WindowStartupLocation.Manual;
                            music.Top = y;
                        }
                    }
                },
                new InputItem
                {
                    Label = "Window.Height:\t",
                    Value = !string.IsNullOrEmpty(panelHeight) ? panelHeight : "100",
                    OnValueChanged = (item, newValue) =>
                    {
                        if (double.TryParse(newValue, out double height))
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            fileSystem.Write("Audio Visualization", "PanelHeight", height.ToString());
                            music.Height = height;
                        }
                    }
                },
                new InputItem
                {
                    Label = "Window.Width:\t",
                    Value = !string.IsNullOrEmpty(panelWidth) ? panelWidth : "300",
                    OnValueChanged = (item, newValue) =>
                    {
                        if (double.TryParse(newValue, out double width))
                        {
                            var music = Application.Current.Resources["Music"] as Music;
                            fileSystem.Write("Audio Visualization", "PanelWidth", width.ToString());
                            music.Width = width;
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