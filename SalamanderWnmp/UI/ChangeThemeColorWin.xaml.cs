using SalamanderWnmp.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// ChangeThemeColor.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeThemeColorWin : Window
    {
        public ChangeThemeColorWin()
        {
            InitializeComponent();
        }

        private void cmdSet_Click(object sender, RoutedEventArgs e)
        {
            var defaultColor = (SolidColorBrush)new BrushConverter().ConvertFromString(Properties.Resources.DefaultThemeColor);
            Application.Current.Resources["ThemeColor"] = defaultColor;
            colorPicker.Color = defaultColor;
        }

        private void colorPicker_ColorChanged(object sender, RoutedPropertyChangedEventArgs<SolidColorBrush> e)
        {
            if (lblColor != null)
                lblColor.Text = "新的颜色是" + e.NewValue.ToString();
            Application.Current.Resources["ThemeColor"] = colorPicker.Color;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Common.Settings.ThemeColor.Value = colorPicker.Color;
            Common.Settings.UpdateSettings();
            this.Close();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Common.Settings.ReadSettings();
            e.Handled = true;
        }
    }
}
