using StackExchange.Redis;
using System;
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
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1"))
            {
                IDatabase db = redis.GetDatabase();
                //db.StringSet("name", "zwj1");
                //db.KeyDelete("name");

                TimeSpan t = new TimeSpan(0, 1, 0);


              
            }
        }
    }
}
