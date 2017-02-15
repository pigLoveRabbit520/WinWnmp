using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// RedisWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RedisWindow : Window
    {

        private static ConnectionMultiplexer redisConn = null;

        public RedisWindow()
        {
            InitializeComponent();
        }

        private void title_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
            e.Handled = true;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            e.Handled = true;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                redisConn = ConnectionMultiplexer.Connect("127.0.0.1");
                IServer server = redisConn.GetServer("127.0.0.1", 6379);
                IEnumerable<RedisKey> keys = server.Keys(pattern: "*");
                lbKeys.ItemsSource = keys;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
