using System;
using System.Collections.Generic;
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
    /// HttpRequester.xaml 的交互逻辑
    /// </summary>
    public partial class HttpRequester : Window
    {
        public HttpRequester()
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

    }
}
