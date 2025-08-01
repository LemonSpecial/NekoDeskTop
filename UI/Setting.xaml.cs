using System.Windows;
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
