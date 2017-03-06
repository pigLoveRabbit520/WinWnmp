using SalamanderWnmp.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

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
            cbMethod.ItemsSource = Enum.GetValues(typeof(RequestMethod));
            cbMethod.SelectedIndex = 0;
        }

        public enum RequestMethod
        {
            GET,
            POST,
            HEAD,
            TRACE,
            PUT,
            DELETE,
            OPTIONS,
            CONNECT
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

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtURL.Text))
            {
                HttpHelper helper = new HttpHelper(txtURL.Text);
                string res = null;
                switch((RequestMethod)cbMethod.SelectedItem)
                {
                    case RequestMethod.GET:
                        res = helper.Get();
                        break;
                    case RequestMethod.POST:
                        res = helper.Post();
                        break;
                }
                this.txtRes.Text = res;
            }
        }


    }
}
