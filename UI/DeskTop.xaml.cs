using NekoDeskTop.Module;
using NekoDeskTop.Neko;
using NekoDeskTop.UI;
using System.IO;
using System.Windows;

namespace NekoDeskTop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WinKeyInterceptor _winKeyInterceptor;

        public MainWindow()
        {
            InitializeComponent();
            InitKey();
            InitINIConfig();
            InitUI();
        }

        public void InitUI()
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Left = SystemParameters.VirtualScreenLeft;
            this.Top = SystemParameters.VirtualScreenTop;
            this.Width = SystemParameters.VirtualScreenWidth;
            this.Height = SystemParameters.VirtualScreenHeight;
            //this.Topmost = true;

            Taskbar taskbar = new Taskbar();
            taskbar.Show();
            taskbar.Dock = DockPosition.Bottom;
        }

        public void InitINIConfig()
        {
            RegistryStartUp registryStartUp = new RegistryStartUp();
            if (!registryStartUp.IsRegisterStartUp())
            {
                registryStartUp.AddRegisterStartUp();
            }

            FileSystem fileSystem = new FileSystem();

            fileSystem.Write("Settings", "Language", "Chinese");
            fileSystem.Write("Settings", "Registry", "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\NekoDeskTop");
            fileSystem.Write("WindowBackgroun-Imag", "Explorer", "C:\\Users\\Public\\Pictures\\bgc.png");
            fileSystem.Write("WindowBackgroun-Imag", "DeskTop", "C:\\Users\\Public\\Videos\\bgc.mp4");
            fileSystem.Write("Server", "IP", "192.168.1.1");

            string path = fileSystem.Read("WindowBackgroun-Imag", "DeskTop");
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                VideoBackground.Source = new Uri(path);
                VideoBackground.MediaEnded += (s, e) =>
                {
                    VideoBackground.Position = TimeSpan.Zero;
                    VideoBackground.Play();
                };
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _winKeyInterceptor?.Dispose();
            base.OnClosed(e);
        }


        public void InitKey()
        {
            _winKeyInterceptor = new WinKeyInterceptor();
            _winKeyInterceptor.WinKeyPressed += OnWinKeyPressed;
        }

        private void OnWinKeyPressed()
        {
            // 显示资源管理器（确保在UI线程执行）
            Dispatcher.Invoke(() =>
            {
                var setting = new Setting { Owner = this };
                setting.Show();
            });
        }

    }
}