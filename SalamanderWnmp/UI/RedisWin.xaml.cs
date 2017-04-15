using SalamanderWnmp.Configuration;
using SalamanderWnmp.Tool;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Common.ConnConfigList = RedisConnHelper.GetConnList();
        }

        private void title_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void btnAddConnect_Click(object sender, RoutedEventArgs e)
        {
            AddRedisConnWin win = new AddRedisConnWin();
            win.Owner = this;
            win.ShowType = 0;
            win.Show();
            win.Closing += AddRedisConnWin_Closing;
            e.Handled = true;
        }

        private void AddRedisConnWin_Closing(object sender, CancelEventArgs e)
        {
            AddRedisConnWin win = sender as AddRedisConnWin;
            if(win.Tag != null && win.Tag.GetType().Name == "RedisConnConfig")
            {
                RedisConnConfig config = win.Tag as RedisConnConfig;
                if(win.ShowType == 0)
                {
                    Common.ConnConfigList.Add(config.ConnName, config);
                }
                else
                {
                    Common.ConnConfigList[config.ConnName] = config;
                }
                RedisConnHelper.WriteConnList(Common.ConnConfigList);
            }
            this.tvDBs.ItemsSource = Common.ConnConfigList;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            //RedisKey key = (RedisKey)lbKeys.SelectedItem;
            //IDatabase db = redisConn.GetDatabase();
        }
    }
}
