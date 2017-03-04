using System;
using System.Windows.Controls;
using SalamanderWnmp.UI;

namespace SalamanderWnmp.Programs
{
    class PHPProgram : WnmpProgram
    {
        public PHPProgram()
        {
            ps.StartInfo.EnvironmentVariables.Add("PHP_FCGI_MAX_REQUESTS", "0"); // Disable auto killing PHP
        }

        private string GetPHPIniPath()
        {
            return MainWin.StartupPath + "/" + Common.Settings.PHPDirName.Value + "/php.ini";
        }


        public override void Start()
        {
            if(this.IsRunning())
            {
                return;
            }
            uint ProcessCount = Common.Settings.PHP_Processes.Value;
            short port = Common.Settings.PHP_Port.Value;
            string phpini = GetPHPIniPath();

            try {
                for (var i = 1; i <= ProcessCount; i++) {
                    StartProcess(exeName, String.Format("-b localhost:{0} -c {1}", port, phpini));
                    Log.wnmp_log_notice("Starting PHP " + i + "/" + ProcessCount + " on port: " + port, progLogSection);
                    port++;
                }
                if(String.IsNullOrEmpty(errOutput))
                {
                    Log.wnmp_log_notice("Started", progLogSection);
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
            this.workingDir = Common.APP_STARTUP_PATH + Common.Settings.PHPDirName.Value;
            this.progLogSection = Log.LogSection.WNMP_PHP;
            this.killStop = true;
            this.confDir = "/php/";
            this.logDir = "/php/logs/";
        }
    }
}
