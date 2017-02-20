using System;
using System.ServiceProcess;
using System.Windows.Controls;

namespace SalamanderWnmp.Programs
{
    class MysqlProgram : WnmpProgram
    {
        private readonly ServiceController mysqlController = new ServiceController();
        public const string ServiceName = "mysql-salamander";

        public MysqlProgram()
        {
            mysqlController.MachineName = Environment.MachineName;
            mysqlController.ServiceName = ServiceName;
        }

        public void RemoveService()
        {
            StartProcess("cmd.exe", stopArgs, true);
        }

        public void InstallService()
        {
            StartProcess(exeName, startArgs, true);
        }

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
                mysqlController.Start();
                //mysqlController.WaitForStatus(ServiceControllerStatus.Running);
                Log.wnmp_log_notice("Started " + progName, progLogSection);
            } catch (Exception ex) {
                Log.wnmp_log_error("Start(): " + ex.Message, progLogSection);
            }
        }

        public override void Stop()
        {
            if(isStopped())
            {
                return;
            }
            try {
                mysqlController.Stop();
                mysqlController.WaitForStatus(ServiceControllerStatus.Stopped);
                Log.wnmp_log_notice("Stopped " + progName, progLogSection);
            } catch (Exception ex) {
                Log.wnmp_log_notice("Stop(): " + ex.Message, progLogSection);
            }
        }


        /// <summary>
        /// 通过ServiceController判断服务是否在运行
        /// </summary>
        /// <returns></returns>
        public override bool IsRunning()
        {
            try
            {
                return mysqlController.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 通过ServiceController判断服务是否停止
        /// </summary>
        /// <returns></returns>
        private bool isStopped()
        {
            return mysqlController.Status == ServiceControllerStatus.Stopped;
        }

        public override void Setup(TextBlock lbl)
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
            this.statusLabel = lbl;
            this.confDir = "/mysql/";
            this.logDir = "/mysql/data/";
        }
    }
}
