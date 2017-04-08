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
    public partial class RedisWin : Window
    {

        private static ConnectionMultiplexer redisConn = null;
        private IServer server = null;

        public RedisWin()
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

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            AddRedisConnWin win = new AddRedisConnWin();
            win.Owner = this;
            win.Show();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            //RedisKey key = (RedisKey)lbKeys.SelectedItem;
            //IDatabase db = redisConn.GetDatabase();
        }
    }
}
