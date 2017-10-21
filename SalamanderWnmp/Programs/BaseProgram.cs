using System;
using System.Diagnostics;
using System.ComponentModel;
using SalamanderWnmp.Tool;

namespace SalamanderWnmp.Programs
{
    public abstract class BaseProgram: INotifyPropertyChanged
    {
        /// <summary>
        /// exe 执行文件位置
        /// </summary>
        public string exeFile { get; set; }

        /// <summary>
        /// 进程名称
        /// </summary>
        public string procName { get; set; }   

        /// <summary>
        /// 进程别名，用来在日志窗口显示
        /// </summary>
        public string programName { get; set; }

        /// <summary>
        /// 进程工作目录（Nginx需要这个参数）
        /// </summary>
        public string workingDir { get; set; }

        /// <summary>
        /// 进程日志前缀
        /// </summary>
        public Log.LogSection progLogSection { get; set; }

        /// <summary>
        /// 进程开启的参数
        /// </summary>
        public string startArgs { get; set; }

        /// <summary>
        /// 关闭进程参数
        /// </summary>
        public string stopArgs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool killStop { get; set; }

        /// <summary>
        /// 进程配置目录
        /// </summary>
        public string confDir { get; set; }

        /// <summary>
        /// 进程日志目录
        /// </summary>
        public string logDir { get; set; }


        /// <summary>
        /// 进程异常退出的记录信息
        /// </summary>
        protected string errOutput = "";



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
            if (IsRunning())
            {
                this.Running = true;
            }
            else
            {
                this.Running = false;
            }
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="exe"></param>
        /// <param name="args"></param>
        /// <param name="wait"></param>
        public void StartProcess(string exe, string args, EventHandler exitedHandler = null)
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
            // ErrorDataReceived event signals each time the process writes a line 
            // to the redirected StandardError stream
            ps.ErrorDataReceived += (sender, e) => {
                errOutput += e.Data;
            };
            ps.Exited += exitedHandler != null ? exitedHandler : (sender, e) => {
                if (!String.IsNullOrEmpty(errOutput))
                {
                    Log.wnmp_log_error("Failed: " + errOutput, progLogSection);
                    errOutput = "";
                }
            };
            ps.Start();

            ps.BeginOutputReadLine();
            ps.BeginErrorReadLine();
        }



        public virtual void Start()
        {
            if(IsRunning())
            {
                return;
            }
            try {
                StartProcess(exeFile, startArgs);
                Log.wnmp_log_notice("Started " + programName, progLogSection);
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
                StartProcess(exeFile, stopArgs);
            var processes = Process.GetProcessesByName(procName);
            foreach (var process in processes) {
                    process.Kill();
            }
            Log.wnmp_log_notice("Stopped " + programName, progLogSection);
        }

        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="procName"></param>
        protected void KillProcess(string procName)
        {
            var processes = Process.GetProcessesByName(procName);
            foreach (var process in processes)
            {
                process.Kill();
            }
        }



        public void Restart()
        {
            this.Stop();
            this.Start();
            Log.wnmp_log_notice("Restarted " + programName, progLogSection);
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
        public abstract void Setup();
    }
}
