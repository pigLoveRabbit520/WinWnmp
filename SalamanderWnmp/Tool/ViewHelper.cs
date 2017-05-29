using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SalamanderWnmp.Tool
{
    class ViewHelper
    {
        public static DependencyObject GetParentView(DependencyObject view)
        {
            return VisualTreeHelper.GetParent(view);
        }
    }
}
