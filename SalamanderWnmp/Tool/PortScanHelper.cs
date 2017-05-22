using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace SalamanderWnmp.Tool
{
    class PortScanHelper
    {
        private static List<PortInfo> portInfoList = new List<PortInfo>();

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
            foreach (var row in rows)
            {
                if(!string.IsNullOrEmpty(row))
                {
                    string newRow = row.Trim();
                    if(newRow.StartsWith("TCP") || newRow.StartsWith("UDP"))
                    {
                        string[] cols = Regex.Split(newRow, "\\s+");
                        if (cols.Length >= 5)
                        {
                            int pid = Int32.Parse(cols[4]);
                            string[] portStr = cols[1].Split(':');
                            PortInfo info = new PortInfo
                            {
                                ProtocolType = cols[0],
                                Port = Int32.Parse(portStr[portStr.Length - 1]),
                                PID = pid,
                                ProcName = Process.GetProcessById(pid).ProcessName
                            };
                            portInfoList.Add(info);
                        }
                    }
                }
            }

            return portInfoList;
        }
    }
}
