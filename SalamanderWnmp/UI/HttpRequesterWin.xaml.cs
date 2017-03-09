using SalamanderWnmp.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// HttpRequester.xaml 的交互逻辑
    /// </summary>
    public partial class HttpRequesterWin : Window
    {
        public HttpRequesterWin()
        {
            InitializeComponent();
            rbtnTxt.IsChecked = true;
            cbMethod.ItemsSource = Enum.GetValues(typeof(RequestMethod));
            cbMethod.SelectedIndex = 0;
            cbHeaderName.ItemsSource = this.frequentHeaderNames;
            this.lbHeaders.ItemsSource = this.headers;
        }


        private LoadingWin winLoding = null;
        private HttpHelper helper = null;
        private ObservableCollection<KeyValuePair<string, string>> headers = new ObservableCollection<KeyValuePair<string, string>>();

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

        private string[] frequentHeaderNames = new string[]
        {
            "Content-Type",
            "Cookie",
            "Host",
            "Referer",
            "User-Agent",
            "Accept",
            "Accept-Charset",
            "Accept-Encoding",
            "Authorization",
            "From",
            "Upgrade"
        };


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
                
                try
                {
                    btnSend.IsEnabled = false;
                    helper = new HttpHelper(txtURL.Text);
                    winLoding = new LoadingWin();
                    winLoding.Owner = this;
                    new Thread(new ParameterizedThreadStart(SendRequest))
                        .Start(cbMethod.SelectedItem);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    btnSend.IsEnabled = true;
                }
               
            }
            e.Handled = true;
        }

        /// <summary>
        /// 发送HTTP请求
        /// </summary>
        /// <param name="method"></param>
        private void SendRequest(object method)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() => 
            {
                winLoding.Show();
            }));
            Thread.Sleep(1000);
            Dictionary<string, string> maps = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                maps.Add(header.Key, header.Value);
            }
            helper.SetHeaders(maps);
            string res = null;
            switch ((RequestMethod)method)
            {
                case RequestMethod.GET:
                    res = helper.Get();
                    break;
                case RequestMethod.POST:
                    helper.SetBody(txtBody.Text);
                    res = helper.Post();
                    break;
            }
            DispatcherHelper.UIDispatcher.Invoke(new Action(() => { txtRes.Text = res; }));
            DispatcherHelper.UIDispatcher.Invoke(new Action(() => 
            {
                winLoding.Close();
                btnSend.IsEnabled = true;
            }));
        }


        private void btnAddHeader_Click(object sender, RoutedEventArgs e)
        {
            string name = cbHeaderName.Text;
            string value = txtHeaderValue.Text;
            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(value))
            {
                int index = -1;
                if(ExistSameKey(name, out index))
                {
                    headers.RemoveAt(index);
                    headers.Add(new KeyValuePair<string, string>(name, value));
                }
                else
                {
                    this.headers.Add(new KeyValuePair<string, string>(name, value));
                }
            }
            e.Handled = true;
        }

        private void btnDeleteHeader_Click(object sender, RoutedEventArgs e)
        {
            if(lbHeaders.SelectedIndex >= 0)
            {
                headers.Remove((KeyValuePair<string, string>)lbHeaders.SelectedItem);
            }
            e.Handled = true;
        }

        /// <summary>
        /// 是否存在相同的key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool ExistSameKey(string key, out int index)
        {
            for(int i = 0; i < headers.Count; i++)
            {
                if (headers[i].Key == key)
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        private void WrapPanel_Checked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
