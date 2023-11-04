using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFastFilter.Utils
{
    public static class JUtinity
    {
        public static void registryStartupChanged(bool isEnable)
        {
            var AppUniqueKey = "JFastFilterApp";
            var pathDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string applicationLocation = System.IO.Path.Combine(pathDirectory, "FastFilter.exe");
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (isEnable)
            {
                if (reg.GetValue(AppUniqueKey) == null)
                {
                    reg.SetValue(AppUniqueKey, applicationLocation, RegistryValueKind.ExpandString);
                }
            }
            else
            {
                reg.DeleteValue(AppUniqueKey);
            }
            reg.Close();
        }
    }
}
