using SalamanderWnmp.Tool;
using SalamanderWnmp.UserClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// PortScan.xaml 的交互逻辑
    /// </summary>
    public partial class PortScanWin : SalamanderWindow
    {
        public PortScanWin()
        {
            InitializeComponent();
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            PortScanHelper.GetPortInfoList();
        }
    }
}
