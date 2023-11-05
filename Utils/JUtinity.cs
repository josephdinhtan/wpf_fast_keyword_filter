using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFastFilter.Utils
{
    public static class JUtinity
    {
        public static string getDirectoryPath()
        {
            var dir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //System.Windows.MessageBox.Show("getDirectoryPath fullLocation: " + dir, "Fast Filter");
            return dir;
        }

        public static void registryStartupChanged(bool isEnable)
        {
            string applicationLocation = "applicationLocation path";
            try
            {
                var AppUniqueKey = "JFastFilterApp";
                applicationLocation = getDirectoryPath() + "\\FastFilter.exe";
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
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Something went wrong.\nRegistryStartupChanged Exception: \n file: " + applicationLocation + "\n" + ex.Message, "Fast Filter");
            }
        }

        public static void openDataBase()
        {
            var filePath = "Unknow file";
            var pathDirectory = "Unknow Directory";
            filePath = getDirectoryPath() + "\\keywords.xml";
            OpenWithDefaultProgram(filePath);
        }
        private static void OpenWithNotepad(string path)
        {
            try
            {
                Process.Start("notepad.exe", path);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("OpenDataBase Exception: \n file: " + path + "\n" + ex.Message, "Fast Filter");
            }
        }

        private static void OpenWithDefaultProgram(string path)
        {
            using Process fileopener = new Process();
            try
            {
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = path;
                fileopener.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("OpenDataBase Exception: \n file: " + path + "\n" + ex.Message, "Fast Filter");
            }
            fileopener.Close();
        }
    }
}
