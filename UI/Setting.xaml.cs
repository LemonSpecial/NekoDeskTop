using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NekoDeskTop.UI
{
    public partial class Setting : Window
    {
        private bool _isInitializing = false;
        private Point _defaultPosition;
        public Setting()
        {
            InitializeComponent();

            StartInitUI();
            InitEnd();
        }

        private void InitEnd()
        {

            if (SettingsMenu.Items.Count > 0)
            {
                _isInitializing = true;
                SettingsMenu.SelectedIndex = 0;
                _isInitializing = false;
                ContentFrame.Navigate(new Uri("/SettingPages/General.xaml", UriKind.Relative));
            }
        }


        private void StartInitUI()
        {
            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.Manual;

            _defaultPosition = new Point(
                (SystemParameters.PrimaryScreenWidth - this.Width) / 2,
                (SystemParameters.PrimaryScreenHeight - this.Height) / 2
            );

            this.Left = _defaultPosition.X;
            this.Top = _defaultPosition.Y;

            FileSystem fileSystem = new FileSystem();
            string path = fileSystem.Read("WindowBackgroun-Imag", "Setting");
            if (!string.IsNullOrEmpty(path))
            {
                this.Background = new ImageBrush(new BitmapImage(new Uri(path)));
            }
            else
            {
                this.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void SettingsMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing || SettingsMenu.SelectedItem == null) return;

            var selectedItem = (ListViewItem)SettingsMenu.SelectedItem;
            var content = ((TextBlock)((StackPanel)selectedItem.Content).Children[0]).Text;

            switch (content)
            {
                case "General":
                    ContentFrame.Navigate(new Uri("/SettingPages/General.xaml", UriKind.Relative));
                    break;
                case "Display":
                    ContentFrame.Navigate(new Uri("/SettingPages/Display.xaml", UriKind.Relative));
                    break;
                case "Plugins":
                    ContentFrame.Navigate(new Uri("/SettingPages/Plugins.xaml", UriKind.Relative));
                    break;
                case "About":
                    ContentFrame.Navigate(new Uri("/SettingPages/About.xaml", UriKind.Relative));
                    break;
            }
        }


        private void RestoreDefaultState()
        {
            _defaultPosition = new Point(
                (SystemParameters.PrimaryScreenWidth - this.ActualWidth) / 2,
                (SystemParameters.PrimaryScreenHeight - this.ActualHeight) / 2
            );

            this.Left = _defaultPosition.X;
            this.Top = _defaultPosition.Y;

            if (SettingsMenu.Items.Count > 0)
            {
                _isInitializing = true;
                SettingsMenu.SelectedIndex = 0;
                _isInitializing = false;
                ContentFrame.Navigate(new Uri("/SettingPages/General.xaml", UriKind.Relative));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            RestoreDefaultState();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
