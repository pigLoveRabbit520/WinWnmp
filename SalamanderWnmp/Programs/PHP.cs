using System;
using SalamanderWnmp.Tool;
using System.Threading;

namespace SalamanderWnmp.Programs
{
    class PHPProgram : BaseProgram
    {

        private const string PHP_CGI_NAME = "php-cgi";
        private const string PHP_MAX_REQUEST = "PHP_FCGI_MAX_REQUESTS";
        private Object locker = new Object();
        private uint FCGI_NUM = 0;
        private bool watchPHPFCGI = true;
        private Thread watchThread;

        private void DecreaseFCGINum()
        {
            lock (locker)
            {
                FCGI_NUM--;
            }
        }

        private void IncreaseFCGINum()
        {
            lock (locker)
            {
                FCGI_NUM++;
            }
        }


        public PHPProgram()
        {
            if (Environment.GetEnvironmentVariable(PHP_MAX_REQUEST) == null)
                Environment.SetEnvironmentVariable(PHP_MAX_REQUEST, "300");
        }

        public override void Start()
        {
            if(!IsRunning() && PortScanHelper.IsPortInUseByTCP(Common.Settings.PHP_Port.Value))
            {
                Log.wnmp_log_error("Port " + Common.Settings.PHP_Port.Value + " is used", progLogSection);
            } 
            else if(!IsRunning())
            {
                for (int i = 0; i < Common.Settings.PHPProcesses.Value; i++)
                {
                    StartProcess(this.exeFile, this.startArgs, (s, args) =>
                    {
                        DecreaseFCGINum();
                    });
                    IncreaseFCGINum();
                }
                WatchPHPFCGINum();
                Log.wnmp_log_notice("Started " + programName, progLogSection);
            }
        }

        /// <summary>
        /// 异步查看php-cgi数量
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private void WatchPHPFCGINum()
        {
            watchPHPFCGI = true;
            watchThread = new Thread(() =>
            {
                while (watchPHPFCGI)
                {
                    uint delta = Common.Settings.PHPProcesses.Value - FCGI_NUM;
                    for (int i = 0; i < delta; i++)
                    {
                        StartProcess(this.exeFile, this.startArgs, (s, args) =>
                        {
                            DecreaseFCGINum();
                        });
                        IncreaseFCGINum();
                        Console.WriteLine("restart a php-cgi");
                    }
                }
            });
            watchThread.Start();
        }

        public void StopWatchPHPFCGINum()
        {
            watchPHPFCGI = false;
        }

        public override void Stop()
        {
            if (!IsRunning())
            {
                return;
            }
            StopWatchPHPFCGINum();
            KillProcess(PHP_CGI_NAME);
            Log.wnmp_log_notice("Stopped " + programName, progLogSection);
        }

        public override void Setup()
        {
            string phpDirPath = Common.APP_STARTUP_PATH + Common.Settings.PHPDirName.Value;

            this.exeFile = string.Format("{0}/php-cgi.exe", phpDirPath);
            this.procName = PHP_CGI_NAME;
            this.programName = "PHP";
            this.workingDir = phpDirPath;
            this.progLogSection = Log.LogSection.WNMP_PHP;
            this.startArgs = String.Format("-b 127.0.0.1:{0} -c {1}/php.ini", Common.Settings.PHP_Port.Value, phpDirPath);
            this.killStop = true;
            this.confDir = "/php/";
            this.logDir = "/php/logs/";
        }
    }
}
