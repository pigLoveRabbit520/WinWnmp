using SalamanderWnmp.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace SalamanderWnmp.Programs
{
    class MysqlProgram : BaseProgram
    {
        private readonly ServiceController mysqlController = new ServiceController();
        public const string ServiceName = "mysql-salamander";

        public MysqlProgram()
        {
            mysqlController.MachineName = Environment.MachineName;
            mysqlController.ServiceName = ServiceName;
        }

        /// <summary>
        /// 移除服务
        /// </summary>
        public void RemoveService()
        {
            StartProcess("cmd.exe", stopArgs, true);
        }

        /// <summary>
        /// 安装服务
        /// </summary>
        public void InstallService()
        {
            StartProcess(exeName, startArgs, true);
        }

        /// <summary>
        /// 获取my.ini中mysql的端口
        /// </summary>
        /// <returns></returns>
        private static int GetIniMysqlListenPort()
        {
            string path = Common.APP_STARTUP_PATH + Common.Settings.MysqlDirName.Value + "/my.ini";
            Regex regPort = new Regex(@"^\s*port\s*=\s*(\d+)");
            Regex regMysqldSec = new Regex(@"^\s*\[mysqld\]");
            using (var sr = new StreamReader(path))
            {
                bool isStart = false;// 是否找到了"[mysqld]"
                string str = null;
                while ((str = sr.ReadLine()) != null)
                {
                    if (isStart && regPort.IsMatch(str))
                    {
                        MatchCollection matches = regPort.Matches(str);
                        foreach (Match match in matches)
                        {
                            GroupCollection groups = match.Groups;
                            if (groups.Count > 1)
                            {
                                try
                                {
                                    return Int32.Parse(groups[1].Value);
                                }
                                catch
                                {
                                    return -1;
                                }
                            }
                        }

                    }
                    // [mysqld]段开始
                    if (regMysqldSec.IsMatch(str))
                    {
                        isStart = true;
                    }
                }
            }
            return -1;
        }


        /// <summary>
        /// 服务是否存在
        /// </summary>
        /// <returns></returns>
        public bool ServiceExists()
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (var service in services) {
                if (service.ServiceName == ServiceName)
                    return true;
            }
            return false;
        }

        public override void Start()
        {
            if (IsRunning())
                return;
            try {
                if (!File.Exists(Common.APP_STARTUP_PATH + Common.Settings.MysqlDirName.Value + "/my.ini"))
                {
                    Log.wnmp_log_error("my.ini file not exist", progLogSection);
                    return;
                }
                int port = GetIniMysqlListenPort();// -1表示提取出错
                if (port != -1 && PortScanHelper.IsPortInUseByTCP(port))
                {
                    Log.wnmp_log_error("Port " + port + " is used", progLogSection);
                    return;
                }
                mysqlController.Start();
                Log.wnmp_log_notice("Started " + progName, progLogSection);
            } catch (Exception ex) {
                Log.wnmp_log_error("Start(): " + ex.Message, progLogSection);
            }
        }

        public override void Stop()
        {
            if(!IsRunning())
            {
                return;
            }
            try {
                mysqlController.Stop();
                mysqlController.WaitForStatus(ServiceControllerStatus.Stopped);
                Log.wnmp_log_notice("Stopped " + progName, progLogSection);
            } catch (Exception ex) {
                Log.wnmp_log_error("Stop(): " + ex.Message, progLogSection);
            }
        }


        /// <summary>
        /// 通过ServiceController判断服务是否在运行
        /// </summary>
        /// <returns></returns>
        public override bool IsRunning()
        {
            mysqlController.Refresh();
            try
            {
                return mysqlController.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                return false;
            }
        }

        public override void Setup()
        {
            this.exeName = Common.APP_STARTUP_PATH + String.Format("{0}/bin/mysqld.exe", Common.Settings.MysqlDirName.Value);
            this.procName = "mysqld";
            this.progName = "mysql";
            this.workingDir = Common.APP_STARTUP_PATH + Common.Settings.MysqlDirName.Value;
            this.progLogSection = Log.LogSection.WNMP_MARIADB;
            this.startArgs = "--install-manual " + MysqlProgram.ServiceName + " --defaults-file=\"" +
                Common.APP_STARTUP_PATH + String.Format("\\{0}\\my.ini\"", Common.Settings.MysqlDirName.Value);
            this.stopArgs = "/c sc delete " + MysqlProgram.ServiceName;
            this.killStop = true;
            this.confDir = "/mysql/";
            this.logDir = "/mysql/data/";
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
