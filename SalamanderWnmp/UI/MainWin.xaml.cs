using SalamanderWnmp.Programs;
using SalamanderWnmp.Tool;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWin : Window, INotifyPropertyChanged
    {
        #region 属性
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly MysqlProgram mysql = new MysqlProgram();
        private readonly WnmpProgram nginx = new NginxProgram();
        private readonly PHPProgram php = new PHPProgram();
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
            AddWinHash();
            ini();
        }

        private void ini()
        {
            // 设置主题颜色
            Application.Current.Resources["ThemeColor"] = Common.Settings.ThemeColor.Value;
            nginx.Setup();
            mysql.Setup();
            php.Setup();
            this.stackNginx.DataContext = nginx;
            this.stackPHP.DataContext = php;
            this.stackMysql.DataContext = mysql;
            this.stackCodePanel.DataContext = this;
            this.stackRedisPanel.DataContext = this;
            this.stackHttpRequester.DataContext = this;
        }

        private void AddWinHash()
        {
            winHash.Add("MenuAbout", "AboutWin");
            winHash.Add("MenuCodePanel", "CodePanelWin");
            winHash.Add("MenuRedis", "RedisWin");
            winHash.Add("MenuSettings", "SettingWin");
            winHash.Add("MenuColor", "ChangeThemeColorWin");
            winHash.Add("MenuHttp", "HttpRequesterWin");
        }

        private void title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
            e.Handled = true;
        }

        private void gridTitle_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.Source;
            switch (btn.Name)
            {
                case "btnExit":
                    Application.Current.Shutdown();
                    break;
                case "btnMinimize":
                    this.WindowState = WindowState.Minimized;
                    break;
                case "btnShowMenu":
                    popupMenu.IsOpen = true;
                    break;
            }
            e.Handled = true;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.setLogComponent(this.txtLog);
            DoCheckIfAppsAreRunningTimer();
            Log.CheckForApps(nginx, mysql, php);
            // 安装mysql服务
            if (Directory.Exists(Constants.APP_STARTUP_PATH + Common.Settings.MysqlDirName.Value))
            {
                if (!mysql.ServiceExists())
                    mysql.InstallService();
            }

            Log.wnmp_log_notice("Wnmp ready to go!", Log.LogSection.WNMP_MAIN);
            // 自动启动
            if (Common.Settings.StartNginxOnLaunch.Value)
                nginx.Start();
            if (Common.Settings.StartMysqlOnLaunch.Value)
                mysql.Start();
            if (Common.Settings.StartPHPOnLaunch.Value)
                php.Start();
        }

        private void DoCheckIfAppsAreRunningTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                nginx.SetStatus();
                mysql.SetStatus();
                php.SetStatus();
                CheckWindowOpenStatus();
            };
            timer.Start();
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
            switch (menuName)
            {
                case "MenuAbout":
                    showWin = new AboutWindow();
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
                case "MenuColor":
                    showWin = new ChangeThemeColorWin();
                    break;
            }
            showWins.Add(showWin);
            showWin.Closing += ChildWindow_Closing;
            showWin.Show();
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
            nginx.Start();
            e.Handled = true;
        }

        private void nginxToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            nginx.Stop();
            e.Handled = true;
        }

        private void phpToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            php.Start();
            e.Handled = true;
        }

        private void phpToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            php.Stop();
            e.Handled = true;
        }

        private void mysqlToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            mysql.Start();
            e.Handled = true;
        }

        private void mysqlToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            mysql.Stop();
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

        private void codePanelToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Window jwin = null;
            if(!HasWindowOpened("MenuCodePanel", ref jwin))
            {
                CodePanelWin win = new CodePanelWin();
                showWins.Add(win);
                win.Closing += ChildWindow_Closing;
                win.Show();
            }
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
            Window winOpened = null;
            if (!HasWindowOpened("MenuRedis", ref winOpened))
            {
                RedisWin win = new RedisWin();
                showWins.Add(win);
                win.Closing += ChildWindow_Closing;
                win.Show();
            }
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
            Window winOpened = null;
            if (!HasWindowOpened("MenuHttp", ref winOpened))
            {
                HttpRequesterWin win = new HttpRequesterWin();
                showWins.Add(win);
                win.Closing += ChildWindow_Closing;
                win.Show();
            }
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

    }
}