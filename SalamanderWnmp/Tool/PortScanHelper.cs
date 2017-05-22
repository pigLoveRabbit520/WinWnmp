using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SalamanderWnmp.Tool
{
    class PortScanHelper
    {
        private static List<PortInfo> portInfoList = null;

        public class PortInfo
        {
            public string ProtocolType { get; set; }

            /// <summary>
            /// 占用端口
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// 进程ID
            /// </summary>
            public int PID { get; set; }

            /// <summary>
            /// 程序名称
            /// </summary>
            public string ProcName { get; set; }
        }

        /// <summary>
        /// 获取端口占用情况
        /// </summary>
        /// <returns></returns>
        public static List<PortInfo> GetPortInfoList()
        {
            Process ps = new Process();
            // 设置命令行、参数    
            ps.StartInfo.FileName = "netstat";
            ps.StartInfo.Arguments = "-ano";
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.RedirectStandardInput = true;
            ps.StartInfo.RedirectStandardOutput = true;
            ps.StartInfo.RedirectStandardError = true;
            ps.StartInfo.CreateNoWindow = true;
            ps.Start();
            string result = ps.StandardOutput.ReadToEnd();
            string[] rows = Regex.Split(result, "\r\n", RegexOptions.IgnoreCase);


            return portInfoList;
        }
    }
}
