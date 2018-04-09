using SalamanderWnmp.UserClass;
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
    public partial class ChangeThemeColorWin : WnmpUI.BaseWindow
    {
        /// <summary>
        /// 修改前的主题颜色
        /// </summary>
        private Brush originBrush = null;

        public ChangeThemeColorWin()
        {
            InitializeComponent();
            originBrush = Common.Settings.ThemeColor.Value;
        }

        private void cmdSet_Click(object sender, RoutedEventArgs e)
        {
            var defaultColor = (SolidColorBrush)new BrushConverter().ConvertFromString(Properties.Resources.DefaultThemeColor);
            Application.Current.Resources["ThemeColor"] = defaultColor;
            colorPicker.Color = defaultColor;
            e.Handled = true;
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
            originBrush = colorPicker.Color;
            this.Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["ThemeColor"] = originBrush;
            this.Close();
        }


    }
}
