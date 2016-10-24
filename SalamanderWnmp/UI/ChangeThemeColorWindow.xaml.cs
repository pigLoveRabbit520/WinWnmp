using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// ChangeThemeColor.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeThemeColorWindow : Window
    {
        public ChangeThemeColorWindow()
        {
            InitializeComponent();
        }

        private void btnChangeThemeColor_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["ThemeColor"] = colorPicker.Color;
        }

        private void cmdSet_Click(object sender, RoutedEventArgs e)
        {
            colorPicker.Color = SystemColors.ActiveCaptionBrush;
        }

        private void colorPicker_ColorChanged(object sender, RoutedPropertyChangedEventArgs<SolidColorBrush> e)
        {
            if (lblColor != null)
                lblColor.Text = "新的颜色是" + e.NewValue.ToString();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
            e.Handled = true;
        }
    }
}
