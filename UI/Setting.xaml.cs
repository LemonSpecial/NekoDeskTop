using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NekoDeskTop.UI
{
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();

            InitUI();
            
        }

        private void InitUI()
        {
            this.WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.AllowsTransparency = true;
            //this.ResizeMode = ResizeMode.CanResize;

            FileSystem fileSystem = new FileSystem();
            string path = fileSystem.Read("WindowBackgroun-Imag", "Explorer");
            if (!string.IsNullOrEmpty(path))
            {
                this.Background = new ImageBrush(new BitmapImage(new Uri(path)));
            }
            else
            {
                this.Background = new SolidColorBrush(Colors.White);
            }
            //SettingsMenu.SelectedIndex = 0;
        }

        private void SettingsMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (SettingsMenu.SelectedItem == null) return;

            var selectedItem = (ListViewItem)SettingsMenu.SelectedItem;
            var content = ((TextBlock)((StackPanel)selectedItem.Content).Children[1]).Text;

            switch (content)
            {
                case "General":
                    ContentFrame.Navigate(new Uri("SettingPages/.xaml", UriKind.Relative));
                    break;
                case "Appearance":
                    ContentFrame.Navigate(new Uri("SettingPages/.xaml", UriKind.Relative));
                    break;
                case "Notifications":
                    ContentFrame.Navigate(new Uri("SettingPages/.xaml", UriKind.Relative));
                    break;
                case "About":
                    ContentFrame.Navigate(new Uri("SettingPages/.xaml", UriKind.Relative));
                    break;
            } 
             */
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
