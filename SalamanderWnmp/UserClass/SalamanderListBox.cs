using SalamanderWnmp.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace SalamanderWnmp.UserClass
{
    /// <summary>
    /// 带平滑滚动的ListBox
    /// </summary>
    class SalamanderListBox: ListBox
    {
        public SalamanderListBox()
        {
            this.AddHandler(ListBox.MouseWheelEvent, new MouseWheelEventHandler(list_MouseWheel), true);
        }


        private void list_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ItemsControl items = (ItemsControl)sender;
            ScrollViewer scroll = ViewHelper.FindVisualChild<ScrollViewer>(items);
            if (scroll != null)
            {
                int d = e.Delta;
                if (d > 0)
                {
                    scroll.LineRight();
                }
                if (d < 0)
                {
                    scroll.LineLeft();
                }
                scroll.ScrollToTop();
            }
        }
    }
}
