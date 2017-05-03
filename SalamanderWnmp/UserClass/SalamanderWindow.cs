using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SalamanderWnmp.UserClass
{
    public class SalamanderWindow : Window
    {
        public SalamanderWindow()
        {
            this.Style = Application.Current.Resources["BaseWindow"] as Style;
        }
    }
}
