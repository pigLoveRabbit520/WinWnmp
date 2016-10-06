using System;
using System.Diagnostics;
using SalamanderWnmp.Configuration;
using SalamanderWnmp.UI;
using System.Windows.Controls;
using System.Windows.Media;

namespace SalamanderWnmp.Programs
{
    public class WnmpProgram
    {
        public TextBlock statusLabel { get; set; } // Label that shows the programs status
        public string exeName { get; set; }    // Location of the executable file
        public string procName { get; set; }   // Name of the process
        public string progName { get; set; }   // User-friendly name of the program 
        public Log.LogSection progLogSection { get; set; } // LogSection of the program
        public string startArgs { get; set; }  // Start Arguments
        public string stopArgs { get; set; }   // Stop Arguments if KillStop is false
        public bool killStop { get; set; }     // Kill process instead of stopping it gracefully
        public string confDir { get; set; }    // Directory where all the programs configuration files are
        public string logDir { get; set; }     // Directory where all the programs log files are
        public Ini Settings { get; set; }
        //public ContextMenuStrip configContextMenu { get; set; } // Displays all the programs config files in |confDir|
        //public ContextMenuStrip logContextMenu { get; set; }    // Displays all the programs log files in |logDir|
  
        public Process ps = new Process();

        public WnmpProgram()
        {
            //configContextMenu = new ContextMenuStrip();
            //logContextMenu = new ContextMenuStrip();
            //configContextMenu.ItemClicked += configContextMenu_ItemClicked;
            //logContextMenu.ItemClicked += logContextMenu_ItemClicked;
        }

        /// <summary>
        /// Changes the labels apperance to started
        /// </summary>
        private void SetStartedLabel()
        {
            statusLabel.Text = "\u221A";
            statusLabel.Foreground = new SolidColorBrush(Colors.Green);
        }

        /// <summary>
        /// Changes the labels apperance to stopped
        /// </summary>
        private void SetStoppedLabel()
        {
            statusLabel.Text = "X";
            statusLabel.Foreground = new SolidColorBrush(Colors.DarkRed);
        }

        public void SetStatusLabel()
        {
            if (this.IsRunning() == true)
                SetStartedLabel();
            else
                SetStoppedLabel();
        }

        public void StartProcess(string exe, string args, bool wait = false)
        {
            ps.StartInfo.FileName = exe;
            ps.StartInfo.Arguments = args;
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.RedirectStandardOutput = true;
            ps.StartInfo.WorkingDirectory = MainWindow.StartupPath + "/nginx";
            ps.StartInfo.CreateNoWindow = true;
            ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ps.Start();

            if (wait) {
                ps.WaitForExit();
            }
        }
        public virtual void Start()
        {
            if(IsRunning())
            {
                Log.wnmp_log_notice(String.Format("{0} is running", progName), 
                    progLogSection);
                return;
            }
            try {
                StartProcess(exeName, startArgs);
                Log.wnmp_log_notice("Started " + progName, progLogSection);
            } catch (Exception ex) {
                Log.wnmp_log_error("Start(): " + ex.Message, progLogSection);
            }
        }

        public virtual void Stop()
        {
            if (killStop == false)
                StartProcess(exeName, stopArgs, true);
            if(!IsRunning())
            {
                Log.wnmp_log_notice(String.Format("{0} is not running", progName), progLogSection);
                return;
            }
            var processes = Process.GetProcessesByName(procName);
            foreach (var process in processes) {
                    process.Kill();
            }
            Log.wnmp_log_notice("Stopped " + progName, progLogSection);
        }

        public void Restart()
        {
            this.Stop();
            this.Start();
            Log.wnmp_log_notice("Restarted " + progName, progLogSection);
        }

        //public void ConfigButton(object sender)
        //{
        //    var btnSender = (Button)sender;
        //    var ptLowerLeft = new Point(0, btnSender.Height);
        //    ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
        //    configContextMenu.Show(ptLowerLeft);
        //}

        //public void LogButton(object sender)
        //{
        //    var btnSender = (Button)sender;
        //    var ptLowerLeft = new Point(0, btnSender.Height);
        //    ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
        //    logContextMenu.Show(ptLowerLeft);
        //}

        //private void configContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        //{
        //    try {
        //        Process.Start(Settings.Editor.Value, frmMain.StartupPath + confDir + e.ClickedItem.Text);
        //    } catch (Exception ex) {
        //        Log.wnmp_log_error(ex.Message, progLogSection);
        //    }
        //}

        //private void logContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        //{
        //    try {
        //        Process.Start(Settings.Editor.Value, frmMain.StartupPath + logDir + e.ClickedItem.Text);
        //    } catch (Exception ex) {
        //        Log.wnmp_log_error(ex.Message, progLogSection);
        //    }
        //}

        public bool IsRunning()
        {
            var processes = Process.GetProcessesByName(procName);

            return (processes.Length != 0);
        }
    }
}
