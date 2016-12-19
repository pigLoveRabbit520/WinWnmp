using System;
using System.ServiceProcess;

namespace SalamanderWnmp.Programs
{
    class MysqlProgram : WnmpProgram
    {
        private readonly ServiceController MysqlController = new ServiceController();
        public const string ServiceName = "mysql-salamander";

        public MysqlProgram()
        {
            MysqlController.MachineName = Environment.MachineName;
            MysqlController.ServiceName = ServiceName;
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
            try
            {
                if (MysqlController.Status == ServiceControllerStatus.Running)
                {
                    return;
                }
            }
            catch(Exception ex)
            {
                Log.wnmp_log_notice("You need to be the administrator to Start Mysql Service", progLogSection);
            }
            try {
                MysqlController.Start();
                MysqlController.WaitForStatus(ServiceControllerStatus.Running);
                Log.wnmp_log_notice("Started " + progName, progLogSection);
            } catch (Exception ex) {
                Log.wnmp_log_error("Start(): " + ex.Message, progLogSection);
            }
        }

        public override void Stop()
        {
            if(MysqlController.Status == ServiceControllerStatus.Stopped)
            {
                return;
            }
            try {
                MysqlController.Stop();
                MysqlController.WaitForStatus(ServiceControllerStatus.Stopped);
                Log.wnmp_log_notice("Stopped " + progName, progLogSection);
            } catch (Exception ex) {
                Log.wnmp_log_notice("Stop(): " + ex.Message, progLogSection);
            }
        }

    }
}
