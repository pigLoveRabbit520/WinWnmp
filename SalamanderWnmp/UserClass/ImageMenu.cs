using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SalamanderWnmp.UserClass
{

    public class ImageMenu : ButtonBase
    {
        static ImageMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageMenu), new FrameworkPropertyMetadata(typeof(ImageMenu)));
        }


        [Description("获取或设置图片")]


        public ImageSource MySource
        {
            get { return (ImageSource)GetValue(MySourceProperty); }
            set { SetValue(MySourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MySource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MySourceProperty =
            DependencyProperty.Register("MySource", typeof(ImageSource), typeof(ImageMenu));





        [Description("获取或设置文本")]


        public String MyText
        {
            get { return (String)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyTextProperty =
            DependencyProperty.Register("MyText", typeof(String), typeof(ImageMenu));


        
    }
}
