using System.IO;
using System.Runtime.InteropServices;
using System.Text;


namespace NekoDeskTop.UI
{
    internal class FileSystem
    {
        private string filePath = AppDomain.CurrentDomain.BaseDirectory + "info.ini";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string? section, string? key, string? val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public FileSystem()
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }

        public string Read(string section, string key, string defaultValue = "")
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, retVal, 255, filePath);
            return retVal.ToString();
        }

        public void DeleteKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, filePath);
        }

        public void DeleteSection(string section)
        {
            WritePrivateProfileString(section, null, null, filePath);
        }

        public bool KeyExists(string section, string key)
        {
            return !string.IsNullOrEmpty(Read(section, key));
        }
    }
}
