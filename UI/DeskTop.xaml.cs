using NekoDeskTop.Module;
using NekoDeskTop.Neko;
using NekoDeskTop.UI;
using System.IO;
using System.Windows;

namespace NekoDeskTop
{
    public partial class MainWindow : Window
    {
        private WinKeyInterceptor _winKeyInterceptor;
        public Music music = new Music();
        TopZ_index topZ_Index = new TopZ_index();
        public Setting setting = new Setting();
        public UI.Console console = new UI.Console();

        public MainWindow()
        {
            InitializeComponent();
            InitKey();
            InitINIConfig();
            InitUI();
            ShareModule();
        }

        private void ShareModule()
        {
            Application.Current.Resources["Music"] = music;
            Application.Current.Resources["TopZ_index"] = topZ_Index;
            Application.Current.Resources["Setting"] = setting;
            Application.Current.Resources["UI.Console"] = console;
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

            if (!fileSystem.KeyExists("Settings", "Language"))
            {
                fileSystem.Write("Settings", "Language", "Chinese");
            }

            if (!fileSystem.KeyExists("Settings", "Registry"))
            {
                fileSystem.Write("Settings", "Registry", "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\NekoDeskTop");
            }

            if (!fileSystem.KeyExists("WindowBackgroun-Imag", "Explorer"))
            {
                fileSystem.Write("WindowBackgroun-Imag", "Explorer", "C:\\Users\\Public\\Pictures\\bgc.png");
            }

            if (!fileSystem.KeyExists("WindowBackgroun-Imag", "Setting"))
            {
                fileSystem.Write("WindowBackgroun-Imag", "Setting", "C:\\Users\\Public\\Pictures\\bgc.png");
            }

            if (!fileSystem.KeyExists("WindowBackgroun-Imag", "DeskTop"))
            {
                fileSystem.Write("WindowBackgroun-Imag", "DeskTop", "C:\\Users\\Public\\Videos\\bgc.mp4");
            }

            if (!fileSystem.KeyExists("Server", "IP"))
            {
                fileSystem.Write("Server", "IP", "192.168.1.1");
            }

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
            Dispatcher.Invoke(() =>
            {
                topZ_Index.SetAbsoluteTopmost(setting);
                setting.Show();
            });
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.F:
                    if (console.IsVisible)
                    {
                        console.Hide();
                    }
                    else
                    {
                        console.Show();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}