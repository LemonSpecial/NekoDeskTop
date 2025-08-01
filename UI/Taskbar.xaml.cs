using System.Windows;
using System.Runtime.InteropServices;

namespace NekoDeskTop.UI
{

    public enum DockPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public partial class Taskbar : Window
    {

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public DockPosition Dock { get; set; } = DockPosition.Bottom;
        public Taskbar()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true;
            this.ShowInTaskbar = false;
            this.MaxHeight = 50;
        }

        private void HideSystemTaskbar()
        {
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
            if (taskbarHandle != IntPtr.Zero)
                ShowWindow(taskbarHandle, SW_HIDE);
        }

        private void ShowSystemTaskbar()
        {
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
            if (taskbarHandle != IntPtr.Zero)
                ShowWindow(taskbarHandle, SW_SHOW);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetWindowPosition();
        }

        private void SetWindowPosition()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            switch (Dock)
            {
                case DockPosition.Top:
                    this.Top = 0;
                    this.Left = 0;
                    this.Width = screenWidth;
                    break;

                case DockPosition.Bottom:
                    this.Top = screenHeight - this.Height;
                    this.Left = 0;
                    this.Width = screenWidth;
                    break;

                case DockPosition.Left:
                    this.Top = 0;
                    this.Left = 0;
                    this.Height = screenHeight;
                    break;

                case DockPosition.Right:
                    this.Top = 0;
                    this.Left = screenWidth - this.Width;
                    this.Height = screenHeight;
                    break;
            }
        }
    }
}
