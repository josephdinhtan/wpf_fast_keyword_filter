using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Wpf_Fast_Filter;
using WpfFastFilter.Utils;

namespace WpfFastFilter.DataBase
{
    public static class DataBaseManager
    {
        private static readonly List<Keywork> keywords = new List<Keywork>();

        internal static List<Keywork> Keywords => keywords;

        public static void readDataBase()
        {
            try
            {
                var xmlPath = JUtinity.getDirectoryPath() + "\\keywords.xml";
                XmlTextReader textReader = new XmlTextReader(xmlPath);
                textReader.Read();
                keywords.Clear();
                while (textReader.Read())
                {
                    // Move to fist element
                    textReader.MoveToElement();
                    if (textReader.Name.Equals("keyword"))
                    {
                        var name = textReader.GetAttribute("name")?.ToString();
                        var tooltip = textReader.GetAttribute("tooltip")?.ToString();
                        var value = textReader.GetAttribute("value")?.ToString();
                        if (name != null && value != null)
                        {
                            Keywords.Add(new Keywork(name, tooltip, value));
                        }
                    }
                }
                textReader.Close();

                Jlog.d("===================");
                Keywords.ForEach(keyword =>
                {
                    Jlog.d("name: " + keyword.name + " tooltip: " + keyword.tooltip + " value: " + keyword.value);
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("ReadDataBase Exception: " + ex.Message, "Fast Filter");
            }
        }



        public static ContextMenu getContextMenusFromKeyword(ContextMenu windowContextMenu)
        {
            var contextMenu = new ContextMenu();
            // add close button
            MenuItem closeItem = new MenuItem();
            closeItem.Header = "Close";
            closeItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("Resources/Close.ico", UriKind.Relative))
            };

            closeItem.Click += (s, e) =>
            {
                windowContextMenu.IsOpen = false;
            };
            contextMenu.Items.Add(closeItem);

            keywords.ForEach(keyword =>
            {
                MenuItem item = new MenuItem();
                item.Header = keyword.name;
                item.ToolTip = keyword.tooltip;
                item.Click += (s, e) =>
                {
                    System.Windows.Clipboard.SetText(keyword.value);
                    SendKeys.SendWait("^{v}");
                };
                contextMenu.Items.Add(item);
            });
            return contextMenu;
        }
    }
}
