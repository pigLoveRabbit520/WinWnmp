using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace SalamanderWnmp.Tool
{
    class PortScanHelper
    {

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
            List<PortInfo> portInfoList = new List<PortInfo>();
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
                            string procName = null;
                            try
                            {
                                procName = Process.GetProcessById(pid).ProcessName;
                            }
                            catch
                            {
                                continue;
                            }
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

        /// <summary>
        /// 通过TCP监听列表判断端口是否被占
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsPortInUseByTCP(int port)
        {
            bool inUse = false;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }
    }
}
