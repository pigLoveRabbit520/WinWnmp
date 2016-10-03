using SalamanderWnmp.Configuration;
using SalamanderWnmp.Programs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
    public partial class MainWindow : Window
    {
        private readonly MysqlProgram mysql = new MysqlProgram();
        private readonly WnmpProgram nginx = new WnmpProgram();
        private readonly PHPProgram php = new PHPProgram();
        // 应用启动目录
        public static string StartupPath { get { return Environment.CurrentDirectory; } }

        public Ini Settings = new Ini();

        public MainWindow()
        {
            InitializeComponent();

            Settings.ReadSettings();
            Settings.UpdateSettings();

            SetupNginx();
            SetupMysql();
            SetupPHP();
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


        private void SetupNginx()
        {
            nginx.Settings = Settings;
            nginx.exeName = StartupPath.Replace(@"\", "/") + "/nginx/nginx.exe";
            nginx.procName = "nginx";
            nginx.progName = "Nginx";
            nginx.progLogSection = Log.LogSection.WNMP_NGINX;
            nginx.startArgs = "";
            nginx.stopArgs = "-s stop";
            nginx.killStop = false;
            nginx.statusLabel = lblNginx;
            nginx.confDir = "/conf/";
            nginx.logDir = "/logs/";
        }

        private void SetupMysql()
        {
            mysql.Settings = Settings;
            mysql.exeName = StartupPath + "/mysql/bin/mysqld.exe";
            mysql.procName = "mysqld";
            mysql.progName = "mysql";
            mysql.progLogSection = Log.LogSection.WNMP_MARIADB;
            string lo = "--install-manual " + MysqlProgram.ServiceName + " --default-file=\"" +
                StartupPath.Replace(@"\", "/") + "/mysql/my.ini\"";
            mysql.startArgs = "--install-manual " + MysqlProgram.ServiceName + " --defaults-file=\"" +
                StartupPath + "\\mysql\\my.ini\"";
            mysql.stopArgs = "/c sc delete " + MysqlProgram.ServiceName;
            mysql.killStop = true;
            mysql.statusLabel = lblMysql;
            mysql.confDir = "/mysql/";
            mysql.logDir = "/mysql/data/";
            if (!mysql.ServiceExists())
                mysql.InstallService();
        }

        public void SetupPHP()
        {
            php.Settings = Settings;
            php.exeName = StartupPath.Replace(@"\", "/") + "/" + php.Settings.phpDirName.Value
                + "/php-cgi.exe";
            php.procName = "php-cgi";
            php.progName = "PHP";
            php.progLogSection = Log.LogSection.WNMP_PHP;
            php.killStop = true;
            php.statusLabel = lblPHP;
            php.confDir = "/php/";
            php.logDir = "/php/logs/";
            //SetCurlCAPath();
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

            Log.wnmp_log_notice("Wnmp ready to go!", Log.LogSection.WNMP_MAIN);

            if (Settings.StartNginxOnLaunch.Value)
                nginx.Start();
            if (Settings.StartMySQLOnLaunch.Value)
                mysql.Start();
            if (Settings.StartPHPOnLaunch.Value)
                php.Start();
        }

        private void DoCheckIfAppsAreRunningTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                nginx.SetStatusLabel();
                mysql.SetStatusLabel();
                php.SetStatusLabel();
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

        private void StackNginx_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.Source;
            switch(btn.Name)
            {
                case "btnOpenNginx":
                    nginx.Start();
                    break;
                case "btnCloseNginx":
                    nginx.Stop();
                    break;
            }
        }

        private void StackPHP_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.Source;
            switch (btn.Name)
            {
                case "btnOpenPHP":
                    php.Start();
                    break;
                case "btnClosePHP":
                    php.Stop();
                    break;
            }
        }

        private void StackMysql_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.Source;
            switch (btn.Name)
            {
                case "btnOpenMysql":
                    mysql.Start();
                    break;
                case "btnCloseMysql":
                    mysql.Stop();
                    break;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageMenu btn = (ImageMenu)e.Source;
            switch (btn.Name)
            {
                case "About":
                    MessageBox.Show("Salamander制作");
                    break;
                case "Output":
                    break;
            }
            e.Handled = true;
        }



    }
}
