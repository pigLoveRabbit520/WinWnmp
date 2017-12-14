using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace SalamanderWnmp.Configuration
{
    /// <summary>
    /// Manages the settings
    /// </summary>
    public class Ini
    {
        public Option<string> Editor { get { return this.editor; } set { this.editor = value; } }
        private Option<string> editor = new Option<string>
        {
            Name = "editor",
            Description = "Editor Path",
            Value = "notepad.exe",
        };

        public Option<Brush> ThemeColor { get { return this.themeColor; } set { this.themeColor = value; } }
        private Option<Brush> themeColor = new Option<Brush>
        {
            Name = "themeColor",
            Description = "Theme Color",
            Value = (Brush)new BrushConverter().ConvertFromString("#16a085"),
        };

        public Option<bool> StartNginxOnLaunch { get { return this.startNginxOnLaunch; } set { this.startNginxOnLaunch = value; } }
        private Option<bool> startNginxOnLaunch = new Option<bool>
        {
            Name = "startNginxOnLaunch",
            Description = "Start Nginx when Wnmp starts",
            Value = false,
        };

        public Option<bool> StartMysqlOnLaunch { get { return this.startMysqlOnLaunch; } set { this.startMysqlOnLaunch = value; } }
        private Option<bool> startMysqlOnLaunch = new Option<bool>
        {
            Name = "startMysqlOnLaunch",
            Description = "Start MySQL when Wnmp starts",
            Value = false,
        };

        public Option<bool> StartPHPOnLaunch { get { return this.startPHPOnLaunch; } set { this.startPHPOnLaunch = value; } }
        private Option<bool> startPHPOnLaunch = new Option<bool>
        {
            Name = "startPhpOnLaunch",
            Description = "Start PHP when Wnmp starts",
            Value = false,
        };

        public Option<bool> MinimizeWnmpToTray { get { return this.minimizeWnmpToTray; } set { this.minimizeWnmpToTray = value; } }
        private Option<bool> minimizeWnmpToTray = new Option<bool>
        {
            Name = "miniMizeWnmpToTray",
            Description = "Minimize to tray instead of minimizing",
            Value = false,
        };


        public Option<string> NginxDirName { get { return this.nginxDirName; } set { this.nginxDirName = value; } }
        private Option<string> nginxDirName = new Option<string>
        {
            Name = "nginxDirName",
            Description = "Nginx directory name",
            Value = "nginx",
        };

        public Option<string> MysqlDirName { get { return this.mysqlDirName; } set { this.mysqlDirName = value; } }
        private Option<string> mysqlDirName = new Option<string>
        {
            Name = "mysqlDirName",
            Description = "Mysql directory name",
            Value = "mysql",
        };

        public Option<string> PHPDirName { get { return this.phpDirName; } set { this.phpDirName = value; } }
        private Option<string> phpDirName = new Option<string>
        {
            Name = "phpDirName",
            Description = "PHP directory name",
            Value = "php",
        };

        public Option<short> PHP_Port { get { return this.php_Port; } set { this.php_Port = value; } }
        private Option<short> php_Port = new Option<short>
        {
            Name = "phpPort",
            Description = "Starting PHP Port",
            Value = 9000,
        };

        /// <summary>
        /// php-cgi开启的个数
        /// </summary>
        public Option<uint> PHPProcesses { get { return this.phpProcesses; } set { this.phpProcesses = value; } }
        private Option<uint> phpProcesses = new Option<uint>
        {
            Name = "phpProcesses",
            Description = "Amount of php-cgi processes",
            Value = 4,
        };

        public Option<string> RedisDirName { get { return this.redisDirName; } set { this.redisDirName = value; } }
        private Option<string> redisDirName = new Option<string>
        {
            Name = "redisDirName",
            Description = "Redis directory name",
            Value = "redis",
        };

        /// <summary>
        /// MySQL命令行用户
        /// </summary>
        public Option<string> MysqlClientUser { get { return this.mysqlClientUser; } set { this.mysqlClientUser = value; } }
        private Option<string> mysqlClientUser = new Option<string>
        {
            Name = "mysqlClientUser",
            Description = "One user of MySQL for logining",
            Value = "root",
        };

        /// <summary>
        /// MySQL命令行用户密码
        /// </summary>
        public Option<string> MysqlClientUserPass { get { return this.mysqlClientUserPass; } set { this.mysqlClientUserPass = value; } }
        private Option<string> mysqlClientUserPass = new Option<string>
        {
            Name = "mysqlClientUserPass",
            Description = "Password for that user",
            Value = "",
        };


        public Option<bool> FirstRun = new Option<bool>
        {
            Name = "firstRun",
            Description = "First run",
            Value = true,
        };


        public Option<bool> StartMinimizedToTray = new Option<bool>
        {
            Name = "startMinimizedToTray",
            Description = "Start Wnmp minimized to tray",
            Value = false,
        };

        private List<IOption> options = new List<IOption>();

        public Ini()
        {
            options.Add(Editor);
            options.Add(ThemeColor);
            options.Add(StartNginxOnLaunch);
            options.Add(StartMysqlOnLaunch);
            options.Add(StartPHPOnLaunch);
            options.Add(StartMinimizedToTray);
            options.Add(MinimizeWnmpToTray);
            options.Add(FirstRun);
            options.Add(PHPProcesses);
            options.Add(PHP_Port);
            options.Add(NginxDirName);
            options.Add(PHPDirName);
            options.Add(MysqlDirName);
            options.Add(RedisDirName);
            options.Add(MysqlClientUser);
            options.Add(mysqlClientUserPass);
        }

        private readonly string IniFile = Constants.APP_STARTUP_PATH + @"\Wnmp.ini";
        private string IniFileStr;
        private bool LoadIniFile()
        {
            if (!File.Exists(IniFile))
                return false;

            using (var sr = new StreamReader(IniFile)) {
                IniFileStr = sr.ReadToEnd();
            }

            return true;
        }

        /// <summary>
        /// Reads the settings from the ini
        /// </summary>
        public void ReadSettings()
        {
            if (!LoadIniFile()) {
                UpdateSettings(); // Add options with default values
                return;
            }

            foreach (var option in options) {
                option.ReadIniValue(IniFileStr);
                option.Convert();
            }

            UpdateSettings();
        }

        /// <summary>
        /// Updates the settings to the ini
        /// </summary>
        public void UpdateSettings()
        {
            using (var sw = new StreamWriter(IniFile)) {
                sw.WriteLine("[WNMP-SALAMANDER]");
                foreach (var option in options) {
                    option.PrintIniOption(sw);
                }
            }
        }
    }
}
