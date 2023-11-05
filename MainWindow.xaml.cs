using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wpf_Fast_Filter.Hotkeys;
using WpfFastFilter.DataBase;
using WpfFastFilter.Properties;
using WpfFastFilter.Utils;

namespace Wpf_Fast_Filter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        private int numberOfHotkeyTriggered = 0;
        private const int PANNEL_CLOSE_TIMER = 10;
        System.Windows.Threading.DispatcherTimer closeKeywordPannelTimer = new System.Windows.Threading.DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();

            // need to setup the global hook. this can go in
            // App.xaml.cs's constructor if you want
            HotkeysManager.SetupSystemHook();

            Closing += MainWindow_Closing;
            StateChanged += MainWindow_StateChanged;

            closeKeywordPannelTimer.Tick += new EventHandler(closeKeywordPannelTimer_Tick);
            closeKeywordPannelTimer.Interval = new TimeSpan(0, 0, PANNEL_CLOSE_TIMER);

            //ContextMenu = Resources["contextMenu"] as ContextMenu;
            HotkeysManager.AddHotkey(ModifierKeys.Control | ModifierKeys.Alt, Key.F, () =>
            {
                numberOfHotkeyTriggered++;
                hotkeyFiredNumber.Content = numberOfHotkeyTriggered;

                ContextMenu.IsOpen = false;
                ContextMenu.IsOpen = true;
                closeKeywordPannelTimer.Stop();
                closeKeywordPannelTimer.Start();
            });

            updateUiValues();
        }

        private void updateUiValues()
        {
            AutoStartupCheckBox.IsChecked = Settings.Default.AutoStartup;
            OpenWhenStartupCheckBox.IsChecked = Settings.Default.OpenWindowWhenStartup;
        }

        private void closeKeywordPannelTimer_Tick(object sender, EventArgs e)
        {
            ContextMenu.IsOpen = false;
            Jlog.d("closeKeywordPannelTimer_Tick");
            closeKeywordPannelTimer.Stop();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Fake close App
            e.Cancel = true;
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
        }

        public void onAppShuttingDown()
        {
            Jlog.d("onAppShuttingDown");

            // Need to shutdown the hook. idk what happens if
            // you dont, but it might cause a memory leak
            HotkeysManager.ShutdownSystemHook();
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                ShowInTaskbar = true;
            }
        }

        private async void readAndUpdateKeywords()
        {
            try
            {
                await Task.Run(() => { DataBaseManager.readDataBase(); });
                ContextMenu = DataBaseManager.getContextMenusFromKeyword(ContextMenu);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Exception: " + ex.Message, "Fast Filter");
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void OpenDbClicked(object sender, RoutedEventArgs e)
        {
            JUtinity.openDataBase();
        }

        private void ReloadDbClicked(object sender, RoutedEventArgs e)
        {
            readAndUpdateKeywords();
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
        }

        private void SaveSettingsClicked(object sender, RoutedEventArgs e)
        {
            if (AutoStartupCheckBox.IsChecked.Value != Settings.Default.AutoStartup)
            {
                JUtinity.registryStartupChanged(AutoStartupCheckBox.IsChecked.Value);
            }
            Settings.Default.OpenWindowWhenStartup = OpenWhenStartupCheckBox.IsChecked.Value;
            Settings.Default.AutoStartup = AutoStartupCheckBox.IsChecked.Value;
            Settings.Default.Save();
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
        }
    }
}
