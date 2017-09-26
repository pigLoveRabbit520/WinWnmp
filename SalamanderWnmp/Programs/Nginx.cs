using SalamanderWnmp.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SalamanderWnmp.Programs
{
    class NginxProgram : BaseProgram
    {

        public override void Setup()
        {
            this.exeFile = Common.APP_STARTUP_PATH + String.Format("{0}/nginx.exe", Common.Settings.NginxDirName.Value);
            this.procName = "nginx";
            this.programName = "Nginx";
            this.workingDir = Common.APP_STARTUP_PATH + Common.Settings.NginxDirName.Value;
            this.progLogSection = Log.LogSection.WNMP_NGINX;
            this.startArgs = "";
            this.stopArgs = "-s stop";
            this.killStop = false;
            this.confDir = "/conf/";
            this.logDir = "/logs/";
        }

        /// <summary>
        /// 打开命令行
        /// </summary>
        public static void OpenNginxtCmd()
        {
            Process ps = new Process();
            ps.StartInfo.FileName = "cmd.exe";
            ps.StartInfo.Arguments = "";
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.CreateNoWindow = false;
            ps.StartInfo.WorkingDirectory = Common.APP_STARTUP_PATH + Common.Settings.NginxDirName.Value;

            ps.Start();

        }


    }
}
