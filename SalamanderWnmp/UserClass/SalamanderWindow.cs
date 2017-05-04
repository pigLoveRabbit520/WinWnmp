using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SalamanderWnmp.UserClass
{
    public class SalamanderWindow : Window
    {
        public SalamanderWindow()
        {
            this.Style = Application.Current.Resources["BaseWindow"] as Style;
        }


        #region 窗口事件函数

        /// <summary>
        /// 窗口移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
            e.Handled = true;
        }

        /// <summary>
        /// 关闭窗口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        #endregion
    }
}
