using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SalamanderWnmp.Programs
{
    class NginxProgram : WnmpProgram
    {

        public override void Setup()
        {
            this.exeName = Common.APP_STARTUP_PATH + String.Format("{0}/nginx.exe", Common.Settings.NginxDirName.Value);
            this.procName = "nginx";
            this.progName = "Nginx";
            this.workingDir = Common.APP_STARTUP_PATH + Common.Settings.NginxDirName.Value;
            this.progLogSection = Log.LogSection.WNMP_NGINX;
            this.startArgs = "";
            this.stopArgs = "-s stop";
            this.killStop = false;
            this.confDir = "/conf/";
            this.logDir = "/logs/";
        }
    }
}
