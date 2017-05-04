using SalamanderWnmp.Configuration;
using SalamanderWnmp.UserClass;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using static SalamanderWnmp.Tool.RedisHelper;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// AddRedisConnWin.xaml 的交互逻辑
    /// </summary>
    public partial class AddRedisConnWin : SalamanderWindow
    {
        /// <summary>
        /// 展示类型，0为添加Window，1为编辑Window
        /// </summary>
        public int ShowType { get; set; }
        private RedisConnConfig connData = new RedisConnConfig();
        private static ConnectionMultiplexer redisConn = null;

        public AddRedisConnWin()
        {
            InitializeComponent();
            if(ShowType == 1)
            {
                this.gridConn.DataContext = this.Tag as RedisConnConfig;
            } 
            else
            {
                this.connData.Port = 6379; // 默认端口
                this.gridConn.DataContext = connData;
            }
        }

        /// <summary>
        /// 连接选项是否完整
        /// </summary>
        /// <returns></returns>
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
