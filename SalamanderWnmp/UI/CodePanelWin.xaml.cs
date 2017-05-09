using SalamanderWnmp.Tool;
using SalamanderWnmp.UserClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// CodePanel.xaml 的交互逻辑
    /// </summary>
    public partial class CodePanelWin : SalamanderWindow
    {


        /// <summary>
        /// 执行代码状态
        /// </summary>
        private enum RunCodeStatus
        {
            Running,
            Stop
        }

        private enum ResizeDirection
        {
            BottomRight = 8,
        }

        private Dictionary<ResizeDirection, Cursor> cursors = new Dictionary<ResizeDirection, Cursor>
        {
            {ResizeDirection.BottomRight, Cursors.SizeNWSE},
        };
       

        public CodePanelWin()
        {
            InitializeComponent();
            this.MouseMove += CodePanelWin_MouseMove;
        }

        private void CodePanelWin_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                FrameworkElement element = e.OriginalSource as FrameworkElement;
                if (element != null && !element.Name.Contains("Resize"))
                    this.Cursor = Cursors.Arrow;
            }
        }

        private void ResizePressed(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            ResizeDirection direction = (ResizeDirection)Enum.Parse(typeof(ResizeDirection), element.Name.Replace("Resize", ""));
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Cursor = Cursors.SizeNWSE;
                ResizeWindow(direction);
            }
            e.Handled = true;
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_HwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        // 选择的编程语言
        private CodeHelper.ProgramLan selectedLan = CodeHelper.ProgramLan.JavaScript;


        public CodeHelper.ProgramLan SelectedLan
        {
            get
            {
                return this.selectedLan;
            }
            set
            {
                this.selectedLan = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedLan"));
                }
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            string code = this.txtCode.Text;
            if(string.IsNullOrEmpty(code))
            {
                MessageBox.Show("请输入代码");
                return;
            }
            CodeHelper helper = new CodeHelper();
            helper.SetPreWork(() =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action<RunCodeStatus>(SetRunButtonContent),
                    RunCodeStatus.Running);
            }).SetAfterWork((result) =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action<String>(ChangOutputTxt), result);
                DispatcherHelper.UIDispatcher.Invoke(new Action<RunCodeStatus>(SetRunButtonContent),
                    RunCodeStatus.Stop);
            }).Run(code, SelectedLan);
        }


        /// <summary>
        /// 更改运行按钮的Content
        /// </summary>
        /// <param name="status"></param>
        private void SetRunButtonContent(RunCodeStatus status)
        {
            switch(status)
            {
                case RunCodeStatus.Running:
                    this.btnRun.Content = "执行中...";
                    this.btnRun.IsEnabled = false;
                    break;
                case RunCodeStatus.Stop:
                    this.btnRun.Content = "运行";
                    this.btnRun.IsEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// 改变状态文本
        /// </summary>
        /// <param name="txt"></param>
        private void ChangOutputTxt(String txt)
        {
            this.txtOutput.Text = txt;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.lanList.DataContext = this;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtOutput.Text = "";            
        }

    }
}
