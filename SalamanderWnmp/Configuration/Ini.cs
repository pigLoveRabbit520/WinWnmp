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
        public Option<string> Editor = new Option<string> {
            Name = "editor", Description = "Editor Path", Value = "notepad.exe",
        };
        public Option<Brush> ThemeColor = new Option<Brush>
        {
            Name = "themeColor", Description = "Theme Color", Value = (Brush)new BrushConverter().ConvertFromString("#16a085"),
        };
        public Option<bool> StartNginxOnLaunch = new Option<bool> {
            Name = "startNginxOnLaunch", Description = "Start Nginx when Wnmp starts", Value = false,
        };
        public Option<bool> StartMySQLOnLaunch = new Option<bool> {
            Name = "startMysqlOnLaunch", Description = "Start MySQL when Wnmp starts", Value = false,
        };
        public Option<bool> StartPHPOnLaunch = new Option<bool> {
            Name = "startPhpOnLaunch", Description = "Start PHP when Wnmp starts", Value = false,
        };
        public Option<bool> MinimizeWnmpToTray = new Option<bool> {
            Name = "miniMizeWnmpToTray", Description = "Minimize to tray instead of minimizing", Value = false,
        };
        public Option<uint> UpdateFrequency = new Option<uint> {
            Name = "updateFrequency", Description = "Update frequency(In days)", Value = 7,
        };
        public Option<string> phpDirName = new Option<string> {
            Name = "phpDirName", Description = "PHP directory name", Value = "php",
        };
        public Option<short> PHP_Port = new Option<short> {
            Name = "phpPort", Description = "Starting PHP Port", Value = 9000,
        };
        public Option<uint> PHP_Processes = new Option<uint> {
            Name = "phpProcesses", Description = "Amount of PHP processes", Value = 2,
        };
        public Option<bool> FirstRun = new Option<bool> {
            Name = "firstRun", Description = "First run", Value = true,
        };
        public Option<bool> MinimizeInsteadOfClosing = new Option<bool> {
            Name = "miniMizeInsteadOfClosing", Description = "Minimize to tray instead of closing", Value = false,
        };
        public Option<bool> StartMinimizedToTray = new Option<bool> {
            Name = "startMinimizedToTray", Description = "Start Wnmp minimized to tray", Value = false,
        };

        private List<IOption> options = new List<IOption>();

        public Ini()
        {
            options.Add(Editor);
            options.Add(ThemeColor);
            options.Add(StartNginxOnLaunch);
            options.Add(StartMySQLOnLaunch);
            options.Add(StartPHPOnLaunch);
            options.Add(StartMinimizedToTray);
            options.Add(MinimizeWnmpToTray);
            options.Add(MinimizeInsteadOfClosing);
            options.Add(FirstRun);
            options.Add(UpdateFrequency);
            options.Add(PHP_Processes);
            options.Add(PHP_Port);
            options.Add(phpDirName);
        }

        private readonly string IniFile = UI.MainWindow.StartupPath + @"\Wnmp.ini";
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
