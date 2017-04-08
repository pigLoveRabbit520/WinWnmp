using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// AddRedisConnWin.xaml 的交互逻辑
    /// </summary>
    public partial class AddRedisConnWin : Window
    {

        public class RedisConn
        {
            /// <summary>
            /// 连接名称
            /// </summary>
            public string ConnName { get; set; }

            /// <summary>
            /// 主机名
            /// </summary>
            public string Host { get; set; }

            /// <summary>
            /// 端口号
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// 验证
            /// </summary>
            public string Auth { get; set; }
        }

        private RedisConn connData = new RedisConn();
        private static ConnectionMultiplexer redisConn = null;

        public AddRedisConnWin()
        {
            InitializeComponent();
            this.connData.Port = 6379; // 默认端口
            this.gridConn.DataContext = connData;
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


        private bool isComplete()
        {
            if(String.IsNullOrEmpty(this.connData.ConnName))
            {
                MessageBox.Show("请填写连接名称");
                return false;
            }
            if (String.IsNullOrEmpty(this.connData.Host))
            {
                MessageBox.Show("请填写主机名");
                return false;
            }
            if (this.connData.Port <= 0)
            {
                MessageBox.Show("请输入端口");
                return false;
            }
            return true;
        }

        private void btnTestConn_Click(object sender, RoutedEventArgs e)
        {
            if(this.isComplete())
            {
                try
                {
                    redisConn = ConnectionMultiplexer.Connect(this.connData.Host +
                        ":" + this.connData.Port);
                    MessageBox.Show("连接成功！");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("连接失败\r\n" + ex.Message);
                }
            }
            e.Handled = true;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this.isComplete())
            {
                this.Tag = this.connData;
                this.Close();
            }
            e.Handled = true;
        }
    }
}
