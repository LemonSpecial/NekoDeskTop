using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

                    string appPath = Assembly.GetExecutingAssembly().Location;
                    key.SetValue(ProgramConfig.AppName, appPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加开机启动失败: {ex.Message}");
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
                Console.WriteLine($"移除开机启动失败: {ex.Message}");
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
