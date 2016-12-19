using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// JSPanel.xaml 的交互逻辑
    /// </summary>
    public partial class JSPanel : Window
    {
        public JSPanel()
        {
            InitializeComponent();
        }

        private void title_MouseDown(object sender, MouseButtonEventArgs e)
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


        //[DllImport("SM.dll", EntryPoint = "runJS", CharSet = CharSet.Ansi,
        //  CallingConvention = CallingConvention.Cdecl)]
        //extern static IntPtr runJS(StringBuilder str);

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //IntPtr intPtr = runJS(new StringBuilder(this.txtCode.Text));
            //string str = Marshal.PtrToStringAnsi(intPtr);
            //this.txtOutput.Text = str;
        }

        private void txtCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Tab)
            {
                int index = txtCode.SelectionStart;
                //txtCode.
            }
            e.Handled = true;
        }
    }
}
