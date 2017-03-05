using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
                txtRes.Text = GetUrltoHtml(txtURL.Text);
            }
        }

        public static string GetUrltoHtml(string Url, string type = "UTF-8")
        {
            try
            {
                WebRequest request = WebRequest.Create(Url);
                System.Net.WebResponse wResp = request.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (StreamReader reader = new StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {

            }
            return "";
        }
    }
}
