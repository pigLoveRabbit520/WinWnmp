using System;
using System.Diagnostics;
using SalamanderWnmp.Configuration;
using SalamanderWnmp.UI;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace SalamanderWnmp.Programs
{
    public abstract class WnmpProgram: INotifyPropertyChanged
    {
        public TextBlock statusLabel { get; set; } // Label that shows the programs status
        public string exeName { get; set; }    // Location of the executable file
        public string procName { get; set; }   // Name of the process
        public string progName { get; set; }   // User-friendly name of the program 
        public string workingDir { get; set; }   // working directory
        public Log.LogSection progLogSection { get; set; } // LogSection of the program
        public string startArgs { get; set; }  // Start Arguments
        public string stopArgs { get; set; }   // Stop Arguments if KillStop is false
        public bool killStop { get; set; }     // Kill process instead of stopping it gracefully
        public string confDir { get; set; }    // Directory where all the programs configuration files are
        public string logDir { get; set; }     // Directory where all the programs log files are

        protected string errOutput = ""; // Output when start the process fail



        public Process ps = new Process();

        public event PropertyChangedEventHandler PropertyChanged;
        // 是否在运行
        private bool running = false;

        public bool Running
        {
            get
            {
                return this.running;
            }
            set
            {
                this.running = value;
                if(PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Running"));
                }
            }
        }


        /// <summary>
        /// 设置状态
        /// </summary>
        public void SetStatus()
        {
            if (this.IsRunning() == true)
            {
                this.Running = true;
            }
            else
            {
                this.Running = false;
            }
               
        }

        public void StartProcess(string exe, string args, bool wait = false)
        {
            ps = new Process();
            ps.StartInfo.FileName = exe;
            ps.StartInfo.Arguments = args;
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.RedirectStandardOutput = true;
            ps.StartInfo.RedirectStandardError = true;
            ps.StartInfo.WorkingDirectory = workingDir;
            ps.StartInfo.CreateNoWindow = true;
            ps.EnableRaisingEvents = true;
            ps.ErrorDataReceived += (sender, e) => {
                errOutput += e.Data;
            };
            ps.Exited += (sender, e) => {
                if(!String.IsNullOrEmpty(errOutput))
                {
                    Log.wnmp_log_error("Failed: " + errOutput, progLogSection);
                    errOutput = "";
                }
            };
            ps.Start();

            ps.BeginOutputReadLine();
            ps.BeginErrorReadLine();


            if (wait) {
                ps.WaitForExit();
                ps.Close();
            }
        }


        /// ErrorDataReceived event signals each time the process writes a line 
        /// to the redirected StandardError stream


        public virtual void Start()
        {
            if(IsRunning())
            {
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
            if(!IsRunning())
            {
                return;
            }
            if (killStop == false)
                StartProcess(exeName, stopArgs, true);
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

        /// <summary>
        /// 判断程序是否运行
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRunning()
        {
            var processes = Process.GetProcessesByName(procName);
            return (processes.Length != 0);
        }

        /// <summary>
        /// 设置初始参数
        /// </summary>
        /// <param name="lbl"></param>
        public abstract void Setup(TextBlock lbl);
    }
}
