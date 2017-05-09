using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace SalamanderWnmp.UserClass
{
    public class SalamanderWindow : Window
    {
        public SalamanderWindow()
        {
            this.Style = Application.Current.Resources["BaseWindow"] as Style;
            this.SourceInitialized += delegate (object sender, EventArgs e)
            {
                this._HwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            };
        }

        protected const int WM_SYSCOMMAND = 0x112;

        /// <summary>
        /// 窗口句柄源
        /// </summary>
        protected HwndSource _HwndSource;


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

        /// <summary>
        /// 设置目标窗体大小，位置
        /// </summary>
        /// <param name="hWnd">目标句柄</param>
        /// <param name="x">目标窗体新位置X轴坐标</param>
        /// <param name="y">目标窗体新位置Y轴坐标</param>
        /// <param name="nWidth">目标窗体新宽度</param>
        /// <param name="nHeight">目标窗体新高度</param>
        /// <param name="BRePaint">是否刷新窗体</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);


        /// <summary>
        /// 返回指定窗口的边框矩形的尺寸。该尺寸以相对于屏幕坐标左上角的屏幕坐标给出
        /// </summary>
        /// <param name="hwnd">目标句柄</param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rect lpRect);

        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    }
}
