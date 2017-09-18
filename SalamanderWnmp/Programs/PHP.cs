using System;
using System.Windows.Controls;
using SalamanderWnmp.UI;
using SalamanderWnmp.Tool;

namespace SalamanderWnmp.Programs
{
    class PHPProgram : BaseProgram
    {
        /// <summary>
        /// fastcgi管理器
        /// 源码：https://github.com/Nazi-Lucy/php-cgi-spawner
        /// </summary>
        private const string FPM_NAME = "php-cgi-spawner";

        private const string PHP_CGI_NAME = "php-cgi";


        public PHPProgram()
        {
            ps.StartInfo.EnvironmentVariables.Add("PHP_HELP_MAX_REQUESTS", "100");
        }

        public override void Start()
        {
            if(!IsRunning() && PortScanHelper.IsPortInUseByTCP(Common.Settings.PHP_Port.Value))
            {
                Log.wnmp_log_error("Port " + Common.Settings.PHP_Port.Value + " is used", progLogSection);
            } 
            else
            {
                base.Start();
            }
        }



        public override void Stop()
        {
            if (!IsRunning())
            {
                return;
            }
            KillProcess(FPM_NAME);
            KillProcess(PHP_CGI_NAME);
            Log.wnmp_log_notice("Stopped " + progName, progLogSection);
        }

        public override void Setup()
        {
            this.exeName = Common.APP_STARTUP_PATH + FPM_NAME; // exe设置为fastcgi管理器程序
            this.procName = FPM_NAME;
            this.progName = "PHP";
            this.workingDir = Common.APP_STARTUP_PATH;
            this.progLogSection = Log.LogSection.WNMP_PHP;
            this.startArgs = String.Format("\"{0} -c {1}\" {2} {3}+10", Common.Settings.PHPDirName.Value + "/php-cgi.exe",
                        Common.Settings.PHPDirName.Value + "/php.ini", Common.Settings.PHP_Port.Value, Common.Settings.PHPProcesses.Value);
            this.killStop = true;
            this.confDir = "/php/";
            this.logDir = "/php/logs/";
        }
    }
}
