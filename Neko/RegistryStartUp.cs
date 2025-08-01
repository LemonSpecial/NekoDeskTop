using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;

namespace NekoDeskTop.Neko
{
    public static class ProgramConfig
    {
        public const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        public const string AppName = "NekoDeskTop";
    }

    internal class RegistryStartUp
    {
        public void AddRegisterStartUp()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(ProgramConfig.RegistryPath, true))
                {
                    if (key == null) return;

                    string appPath = Process.GetCurrentProcess().MainModule.FileName;
                    key.SetValue(ProgramConfig.AppName, $"\"{appPath}\"");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add Error: {ex.Message}");
            }
        }

        public void RemoveRegisterStartUp()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(ProgramConfig.RegistryPath, true))
                {
                    if (key == null) return;

                    if (key.GetValue(ProgramConfig.AppName) != null)
                    {
                        key.DeleteValue(ProgramConfig.AppName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Remove Error: {ex.Message}");
            }
        }

        public bool IsRegisterStartUp()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(ProgramConfig.RegistryPath, false))
                {
                    return key?.GetValue(ProgramConfig.AppName) != null;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
