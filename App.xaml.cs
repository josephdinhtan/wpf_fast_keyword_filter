using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using Forms = System.Windows.Forms;
using System.Windows.Controls;
using System.Diagnostics;
using WpfFastFilter.DataBase;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Threading;
using WpfFastFilter.Utils;
using WpfFastFilter.Properties;

namespace Wpf_Fast_Filter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : System.Windows.Application
    {
        private readonly Forms.NotifyIcon _notifyIcon;
        private readonly MainWindow _mainWindow;

        public App()
        {
            _notifyIcon = new Forms.NotifyIcon();
            _mainWindow = new MainWindow();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool aIsNewInstance = false;
            var myMutex = new Mutex(true, "JosephFastFilter", out aIsNewInstance);
            if (!aIsNewInstance)
            {
                System.Windows.MessageBox.Show("  Another instance is running.\n  Please press \"OK\" and check Window Notification area", "Fast Filter", MessageBoxButton.OK, MessageBoxImage.Information);
                App.Current.Shutdown();
                return;
            }

            _notifyIcon.Icon = new System.Drawing.Icon("Resources/FastFilter.ico");
            _notifyIcon.Text = "Fast filter";
            _notifyIcon.DoubleClick += OpenAppActionClicked;


            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Open", null, OpenAppActionClicked);
            _notifyIcon.ContextMenuStrip.Items.Add("Close", null, CloseAppActionClicked);
            _notifyIcon.Visible = true;
            if (Settings.Default.OpenWindowWhenStartup)
            {
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.ShowInTaskbar = true;
            }
            else
            {
                _mainWindow.WindowState = WindowState.Minimized;
                _mainWindow.ShowInTaskbar = false;
            }
            _mainWindow.Show();

            // Init item to make sure ContextMenu available
            var contextMenu = new ContextMenu();
            MenuItem wrongInitItem = new MenuItem();
            wrongInitItem.Header = "Something Went Wrong";

            contextMenu.Items.Add(wrongInitItem);
            MainWindow.ContextMenu = contextMenu;

            readAndUpdateKeywords();
            base.OnStartup(e);
        }


        private async void readAndUpdateKeywords()
        {
            try
            {
                await Task.Run(() => { DataBaseManager.readDataBase(); });
                MainWindow.ContextMenu = DataBaseManager.getContextMenusFromKeyword(MainWindow.ContextMenu);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Exception: " + ex.Message, "Fast Filter");
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void OpenAppActionClicked(object sender, EventArgs e)
        {
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }

        private void CloseAppActionClicked(object sender, EventArgs e)
        {
            Jlog.d("CloseAppActionClicked");
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Jlog.d("OnExit");
            _notifyIcon.Dispose();
            _mainWindow.onAppShuttingDown();
            base.OnExit(e);
        }
    }
}
