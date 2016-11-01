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
        public Ini Settings = new Ini();
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
            Settings.ReadSettings();
            PHPConfigurationMgr.LoadPHPExtensions(Settings.phpDirName.Value);
            lbPHPExt.ItemsSource = PHPConfigurationMgr.GetExtensions();
            txtTotal.Text = String.Format("(总{0}项)", lbPHPExt.Items.Count);
            e.Handled = true;
        }

        private void btnSaveRegular_Click(object sender, RoutedEventArgs e)
        {
            Settings.UpdateSettings();
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
