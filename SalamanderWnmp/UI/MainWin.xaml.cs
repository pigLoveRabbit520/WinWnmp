using SalamanderWnmp.Configuration;
using SalamanderWnmp.Programs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWin : Window
    {
        private readonly MysqlProgram mysql = new MysqlProgram();
        private readonly WnmpProgram nginx = new NginxProgram();
        private readonly PHPProgram php = new PHPProgram();


        // 应用启动目录
        public static string StartupPath { get { return Constants.APP_STARTUP_PATH; } }

        // 显示的界面集合
        private List<Window> showWins = new List<Window>();
        // 
        Hashtable winHash = new Hashtable();

        /// <summary>
        /// 主线程ID
        /// </summary>
        public static readonly int MainThreadId = Thread.CurrentThread.ManagedThreadId;

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
        }

        private void AddWinHash()
        {
            winHash.Add("MenuAbout", "AboutWindow");
            winHash.Add("MenuJSPanel", "CodePanel");
            winHash.Add("MenuRedis", "RedisWindow");
            winHash.Add("MenuSettings", "SettingWindow");
            winHash.Add("MenuColor", "ChangeThemeColorWindow");
            winHash.Add("MenuHttp", "HttpRequesterWindow");
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


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string path);

        private void SetCurlCAPath()
        {
            var phpini = StartupPath + "/php5.6/php.ini";

            string[] file = File.ReadAllLines(phpini);
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i].Contains("curl.cainfo") == false)
                    continue;

                Regex reg = new Regex("\".*?\"");
                string replace = "\"" + StartupPath + @"\contrib\cacert.pem" + "\"";
                file[i] = file[i].Replace(reg.Match(file[i]).ToString(), replace);
            }
            using (var sw = new StreamWriter(phpini))
            {
                foreach (var line in file)
                    sw.WriteLine(line);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.setLogComponent(this.txtLog);
            DoCheckIfAppsAreRunningTimer();
            CheckForApps();
            // 安装mysql服务
            if (Directory.Exists(StartupPath + Common.Settings.MysqlDirName.Value))
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
            };
            timer.Start();
        }

        /// <summary>
        /// 判断PHP，mysql，nginx是否在wnmp目录中
        /// </summary>
        private void CheckForApps()
        {
            Log.wnmp_log_notice("Checking for applications", Log.LogSection.WNMP_MAIN);
            if (!File.Exists(StartupPath + "/nginx/nginx.exe"))
                Log.wnmp_log_error("Error: Nginx Not Found", Log.LogSection.WNMP_NGINX);

            if (!Directory.Exists(StartupPath + "/mysql"))
                Log.wnmp_log_error("Error: Mysql Not Found", Log.LogSection.WNMP_MARIADB);

            if (!Directory.Exists(StartupPath + "/php"))
                Log.wnmp_log_error("Error: PHP Not Found", Log.LogSection.WNMP_PHP);
        }


        /// <summary>
        /// window是否已经打开
        /// </summary>
        /// <param name="btnName"></param>
        /// <param name="openWin">打开的window</param>
        /// <returns></returns>
        private bool HasWindowOpened(string btnName, ref Window openWin)
        {
            if (showWins.Count > 0)
            {
                foreach(Window win in showWins)
                {
                    if (win.GetType().Name == winHash[btnName].ToString())
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
        /// <param name="btnName"></param>
        private void OpenWindow(string btnName)
        {
            Window showWin = null;
            switch (btnName)
            {
                case "MenuAbout":
                    showWin = new AboutWindow();
                    break;
                case "MenuJSPanel":
                    showWin = new CodePanelWin();
                    break;
                case "MenuRedis":
                    showWin = new RedisWin();
                    break;
                case "MenuSettings":
                    showWin = new SettingWin();
                    break;
                case "MenuHttp":
                    showWin = new HttpRequester();
                    break;
                case "MenuColor":
                    showWin = new ChangeThemeColorWin();
                    break;
            }
            showWins.Add(showWin);
            showWin.Closing += ChildWindow_Closing;
            showWin.Show();
        }

        private void ChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Window win = (Window)sender;
            showWins.Remove(win);
        }




        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageMenu btn = (ImageMenu)e.Source;

            if(btn.Name == "MenuDir")
            {
                Process.Start("explorer.exe", StartupPath);
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

    }
}
