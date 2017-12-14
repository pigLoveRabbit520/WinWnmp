using SalamanderWnmp.Tool;
using System;
using System.Diagnostics;
using System.Windows;

namespace SalamanderWnmp.Programs
{
    class RedisProgram : BaseProgram
    {
        public override void Setup()
        {
            this.exeFile = Common.APP_STARTUP_PATH + String.Format("{0}/redis-server.exe", Common.Settings.RedisDirName.Value);
            this.procName = "redis-server";
            this.programName = "Redis";
            this.workingDir = Common.APP_STARTUP_PATH + Common.Settings.RedisDirName.Value;
            this.progLogSection = Log.LogSection.WNMP_REDIS;
            this.startArgs = "redis.windows.conf";
            this.stopArgs = "";
            this.killStop = true;
            this.confDir = "";
            this.logDir = "";
        }

        /// <summary>
        /// 打开MySQL Client命令行
        /// </summary>
        public static void OpenMySQLClientCmd()
        {
            Process ps = new Process();
            ps.StartInfo.FileName = Common.APP_STARTUP_PATH + String.Format("{0}/bin/mysql.exe", Common.Settings.MysqlDirName.Value);
            ps.StartInfo.Arguments = String.Format("-u{0} -p{1}", Common.Settings.MysqlClientUser.Value, Common.Settings.MysqlClientUserPass.Value);
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.CreateNoWindow = false;

            ps.Start();
        }

    }
}
