using System;
using System.Windows.Controls;
using SalamanderWnmp.UI;
using SalamanderWnmp.Tool;

namespace SalamanderWnmp.Programs
{
    class PHPProgram : WnmpProgram
    {
        /// <summary>
        /// fastcgi管理器
        /// </summary>
        private string FPM_Name = "php-cgi-spawner";

        public PHPProgram()
        {
            ps.StartInfo.EnvironmentVariables.Add("PHP_FCGI_MAX_REQUESTS", "100"); // Disable auto killing PHP
        }



        public override void Start()
        {
            if(this.IsRunning())
            {
                return;
            }
            uint ProcessCount = Common.Settings.PHP_Processes.Value;
            short port = Common.Settings.PHP_Port.Value;

            try {
                string args = String.Format("\"{0} -c {1}\" {2} {3}+5", Common.Settings.PHPDirName.Value + "/php-cgi.exe",
                        Common.Settings.PHPDirName.Value + "/php.ini", port, ProcessCount);
                // 开启PHP-FPM
                StartProcess(Common.APP_STARTUP_PATH + this.FPM_Name, args);
                Log.wnmp_log_notice("Starting PHP " + " on port: " + port, progLogSection);
                if (String.IsNullOrEmpty(errOutput))
                {
                    //Log.wnmp_log_notice("Started", progLogSection);
                }
                else
                {
                    Log.wnmp_log_error("Start(): " + errOutput, progLogSection);
                }
            } catch (Exception ex) {
                Log.wnmp_log_error("Start(): " + ex.Message, progLogSection);
            }
        }

        public override void Setup()
        {
            this.exeName = Common.APP_STARTUP_PATH + Common.Settings.PHPDirName.Value
                + "/php-cgi.exe";
            this.procName = "php-cgi";
            this.progName = "PHP";
            this.workingDir = Common.APP_STARTUP_PATH;
            this.progLogSection = Log.LogSection.WNMP_PHP;
            this.killStop = true;
            this.confDir = "/php/";
            this.logDir = "/php/logs/";
        }
    }
}
