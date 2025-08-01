using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;

namespace NekoDeskTop.Module
{
    internal class WinKeyInterceptor : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_LWIN = 0x5B;
        private const int VK_RWIN = 0x5C;

        private readonly LowLevelKeyboardProc _proc;
        private nint _hookId = nint.Zero;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, nint hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(nint hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint GetModuleHandle(string lpModuleName);

        public event Action WinKeyPressed;
        public event Action WinKeyReleased;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam);

        public WinKeyInterceptor()
        {
            _proc = HookCallback;
            _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, nint.Zero, 0);
            if (_hookId == nint.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private nint HookCallback(int nCode, nint wParam, nint lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == VK_LWIN || vkCode == VK_RWIN)
                {
                    if (wParam == WM_KEYDOWN)
                    {
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            try { WinKeyPressed?.Invoke(); }
                            catch { }
                        });
                    }
                    else if (wParam == WM_KEYUP)
                    {
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            try { WinKeyReleased?.Invoke(); }
                            catch { }
                        });
                    }
                    return 1;
                }
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            if (_hookId != nint.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = nint.Zero;
            }
            GC.SuppressFinalize(this);
        }

        ~WinKeyInterceptor() => Dispose();
    }
}
