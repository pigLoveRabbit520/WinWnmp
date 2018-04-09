using SalamanderWnmp.UserClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    /// QuestionWin.xaml 的交互逻辑
    /// </summary>
    public partial class QuestionsWin : WnmpUI.BaseWindow
    {
        public QuestionsWin()
        {
            InitializeComponent();
        }



        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
