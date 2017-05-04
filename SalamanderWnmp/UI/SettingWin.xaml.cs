using SalamanderWnmp.Configuration;
using SalamanderWnmp.UserClass;
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
    public partial class SettingWin : SalamanderWindow
    {
        private PHPConfigurationManager PHPConfigurationMgr = new PHPConfigurationManager();


        public SettingWin()
        {
            InitializeComponent();
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
            Common.Nginx.Setup();
            Common.Mysql.Setup();
            Common.PHP.Setup();
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
