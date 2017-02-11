using SalamanderWnmp.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static SalamanderWnmp.Configuration.PHPConfigurationManager;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        private PHPConfigurationManager PHPConfigurationMgr = new PHPConfigurationManager();


        public SettingWindow()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            e.Handled = true;
        }

        private void title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gridRegular.DataContext = Common.Settings;
            PHPConfigurationMgr.LoadPHPExtensions(Common.Settings.PHPDirName.Value);
            lbPHPExt.ItemsSource = PHPConfigurationMgr.GetExtensions();
            txtTotal.DataContext = lbPHPExt;
            e.Handled = true;
        }

        private void btnSaveRegular_Click(object sender, RoutedEventArgs e)
        {
            Common.Settings.UpdateSettings();
            this.Close();
            e.Handled = true;
        }

        private void btnSavePHPExt_Click(object sender, RoutedEventArgs e)
        {
            PHPConfigurationMgr.SavePHPIniOptions();
            this.Close();
            e.Handled = true;
        }
    }
}
