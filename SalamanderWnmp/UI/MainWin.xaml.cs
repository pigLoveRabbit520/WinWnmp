using SalamanderWnmp.Programs;
using SalamanderWnmp.Properties;
using SalamanderWnmp.Tool;
using SalamanderWnmp.UserClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWin : SalamanderWindow, INotifyPropertyChanged
    {
        #region 属性
        public event PropertyChangedEventHandler PropertyChanged;
       
        // 显示的界面集合
        private List<Window> showWins = new List<Window>();
        // 
        Hashtable winHash = new Hashtable();
        private bool codePanelOpened = false;
        public bool CodePanelOpened
        {
            get
            {
                return this.codePanelOpened;
            }
            set
            {
                this.codePanelOpened = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CodePanelOpened"));
                }
            }

        }
        private bool redisPanelOpened = false;
        public bool RedisPanelOpened
        {
            get
            {
                return this.redisPanelOpened;
            }
            set
            {
                this.redisPanelOpened = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("RedisPanelOpened"));
                }
            }

        }
        private bool httpRequesterPanelOpened = false;
        public bool HttpRequesterPanelOpened
        {
            get
            {
                return this.httpRequesterPanelOpened;
            }
            set
            {
                this.httpRequesterPanelOpened = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HttpRequesterPanelOpened"));
                }
            }

        }


        #endregion

        public MainWin()
        {
            InitializeComponent();
            Common.Settings.ReadSettings();
            Common.Nginx.Setup();
            Common.Mysql.Setup();
            Common.PHP.Setup();
            if (Common.Settings.FirstRun.Value)
            {
                EnvironmentAutoConfig.Run();
                // 判断mysql服务是否安装过
                if(Common.Mysql.ServiceExists())
                {
                    Common.Mysql.RemoveService();
                }
                Common.Settings.FirstRun.Value = false;
                Common.Settings.UpdateSettings();
            }
            ini();
            this.lbSliderContainer.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(listBoxScrollView_ScrollChanged), true);
        }

        private void ini()
        {
            // 设置主题颜色
            Application.Current.Resources["ThemeColor"] = Common.Settings.ThemeColor.Value;
            AddWinHash();
            this.stackNginx.DataContext = Common.Nginx;
            this.stackPHP.DataContext = Common.PHP;
            this.stackMysql.DataContext = Common.Mysql;
            this.stackCodePanel.DataContext = this;
            this.stackRedisPanel.DataContext = this;
            this.stackHttpRequester.DataContext = this;
        }

        /// <summary>
        /// 添加Hash对应
        /// </summary>
        private void AddWinHash()
        {
            winHash.Add("MenuCodePanel", "CodePanelWin");
            winHash.Add("MenuRedis", "RedisWin");
            winHash.Add("MenuSettings", "SettingWin");
            winHash.Add("MenuColor", "ChangeThemeColorWin");
            winHash.Add("MenuHttp", "HttpRequesterWin");
            winHash.Add("MenuAbout", "AboutWin");
            winHash.Add("MenuQuestions", "QuestionsWin");
            winHash.Add("MenuPortScaner", "PortScanWin");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings s = new Settings();
            s.hMainWnd = (Int64)this._HwndSource.Handle;
            s.Save();
            Log.setLogComponent(this.txtLog);
            DoCheckIfAppsAreRunningTimer();
            Log.CheckForApps(Common.Nginx, Common.Mysql, Common.PHP);
            // 安装mysql服务
            if (Directory.Exists(Constants.APP_STARTUP_PATH + Common.Settings.MysqlDirName.Value))
            {
                if (!Common.Mysql.ServiceExists())
                    Common.Mysql.InstallService();
            }

            Log.wnmp_log_notice("Wnmp ready to go!", Log.LogSection.WNMP_MAIN);
            // 自动启动
            if (Common.Settings.StartNginxOnLaunch.Value)
                Common.Nginx.Start();
            if (Common.Settings.StartMysqlOnLaunch.Value)
                Common.Mysql.Start();
            if (Common.Settings.StartPHPOnLaunch.Value)
                Common.PHP.Start();
        }

        private void DoCheckIfAppsAreRunningTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                Common.Nginx.SetStatus();
                Common.Mysql.SetStatus();
                Common.PHP.SetStatus();
                CheckWindowOpenStatus();
            };
            timer.Start();
        }


        private void gridTitle_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.Source;
            switch (btn.Name)
            {
                case "btnExit":
                    ShowWindowAsync(_HwndSource.Handle, SW_HIDE);
                    break;
                case "btnMinimize":
                    this.WindowState = WindowState.Minimized;
                    break;
                case "btnShowMenu":
                    popupMenu.IsOpen = true;
                    break;
                case "btnChangeThemeColor":
                    OpenWindow("MenuColor");
                    break;
                case "btnOpenMainDir":
                    Process.Start("explorer.exe", Constants.APP_STARTUP_PATH);
                    break;
            }
            e.Handled = true;
        }



        /// <summary>
        /// 检查Window开启情况
        /// </summary>
        private void CheckWindowOpenStatus()
        {
            Window win = null;
            if(HasWindowOpened("MenuCodePanel", ref win))
            {
                this.CodePanelOpened = true;
            }
            else
            {
                this.CodePanelOpened = false;
            }
            if (HasWindowOpened("MenuRedis", ref win))
            {
                this.RedisPanelOpened = true;
            }
            else
            {
                this.RedisPanelOpened = false;
            }
            if (HasWindowOpened("MenuHttp", ref win))
            {
                this.HttpRequesterPanelOpened = true;
            }
            else
            {
                this.HttpRequesterPanelOpened = false;
            }
        }


        /// <summary>
        /// window是否已经打开
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="openWin">打开的window</param>
        /// <returns></returns>
        private bool HasWindowOpened(string menuName, ref Window openWin)
        {
            if (showWins.Count > 0)
            {
                foreach(Window win in showWins)
                {
                    if (win.GetType().Name == winHash[menuName].ToString())
                    {
                        openWin = win;
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 打开window
        /// </summary>
        /// <param name="menuName"></param>
        private void OpenWindow(string menuName)
        {
            Window showWin = null;
            if (!HasWindowOpened(menuName, ref showWin))
            {
                switch (menuName)
                {
                    case "MenuAbout":
                        showWin = new AboutWin();
                        break;
                    case "MenuCodePanel":
                        showWin = new CodePanelWin();
                        break;
                    case "MenuRedis":
                        showWin = new RedisWin();
                        break;
                    case "MenuSettings":
                        showWin = new SettingWin();
                        break;
                    case "MenuHttp":
                        showWin = new HttpRequesterWin();
                        break;
                    case "MenuQuestions":
                        showWin = new QuestionsWin();
                        break;
                    case "MenuColor":
                        showWin = new ChangeThemeColorWin();
                        break;
                    case "MenuPortScaner":
                        showWin = new PortScanWin();
                        break;
                }
                showWins.Add(showWin);
                showWin.Closing += ChildWindow_Closing;
                showWin.Show();
            }
            else
            {
                showWin.Activate();
            }

        }

        private void ChildWindow_Closing(object sender, CancelEventArgs e)
        {
            Window win = (Window)sender;
            showWins.Remove(win);
        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageMenu btn = (ImageMenu)e.Source;

            if(btn.Name == "MenuDir")
            {
                Process.Start("explorer.exe", Constants.APP_STARTUP_PATH);
            }
            else
            {
                Window showWin = null;
                if (HasWindowOpened(btn.Name, ref showWin))
                {
                    showWin.Activate();
                }
                else
                {
                    OpenWindow(btn.Name);
                }
            }
            popupMenu.IsOpen = false;
            e.Handled = true;
        }

        private void nginxToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Common.Nginx.Start();
            e.Handled = true;
        }

        private void nginxToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Common.Nginx.Stop();
            e.Handled = true;
        }

        private void phpToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Common.PHP.Start();
            e.Handled = true;
        }

        private void phpToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Common.PHP.Stop();
            e.Handled = true;
        }

        private void mysqlToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Common.Mysql.Start();
            e.Handled = true;
        }

        private void mysqlToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Common.Mysql.Stop();
            e.Handled = true;
        }

        private void btnSlide_Click(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            switch(btn.Name)
            {
                case "btnSlideBack":
                    lbSliderContainer.ScrollIntoView(areaTwo);
                    break;
                case "btnSlideForward":
                    lbSliderContainer.ScrollIntoView(areaFirst);
                    break;
            }

            e.Handled = true;
        }

        private const double ITEM_WIDTH = 480;

        // ListBox中ScrollView滚动事件监听
        private void listBoxScrollView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sv = e.OriginalSource as ScrollViewer;
            if (sv != null)
            {
                if(sv.HorizontalOffset == 0)
                {
                    btnSlideForward.IsChecked = true;
                }
                else if(sv.HorizontalOffset == ITEM_WIDTH)
                {
                    btnSlideBack.IsChecked = true;
                }
            }
        }

        private void codePanelToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            OpenWindow("MenuCodePanel");
            e.Handled = true;
        }

        private void codePanelToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Window winOpened = null;
            if (HasWindowOpened("MenuCodePanel", ref winOpened))
            {
                winOpened.Close();
                showWins.Remove(winOpened);
            }
            e.Handled = true;
        }


        private void redisPanelToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            OpenWindow("MenuRedis");
            e.Handled = true;
        }

        private void redisPanelToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Window winOpened = null;
            if (HasWindowOpened("MenuRedis", ref winOpened))
            {
                winOpened.Close();
                showWins.Remove(winOpened);
            }
            e.Handled = true;
        }


        private void httpRequesterToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            OpenWindow("MenuHttp");
            e.Handled = true;
        }

        private void httpRequesterToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Window winOpened = null;
            if (HasWindowOpened("MenuHttp", ref winOpened))
            {
                winOpened.Close();
                showWins.Remove(winOpened);
            }
            e.Handled = true;
        }

        private void NotificationAreaIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowWindowAndSetForeground();
        }

        private void MenuItem_Show_Click(object sender, EventArgs e)
        {
            ShowWindowAndSetForeground();
        }

        private void MenuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_About_Click(object sender, EventArgs e)
        {
            OpenWindow("MenuAbout");
        }

        private void MySQL_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MysqlProgram.OpenMySQLClientCmd();
            e.Handled = true;
        }

        private void Nginx_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NginxProgram.OpenNginxtCmd();
            e.Handled = true;
        }
    }
}