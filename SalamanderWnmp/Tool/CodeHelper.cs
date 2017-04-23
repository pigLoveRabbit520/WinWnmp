using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SalamanderWnmp.Tool
{
    public class CodeHelper
    {
        /// <summary>
        /// 编程语言枚举
        /// </summary>
        public enum ProgramLan
        {
            JavaScript,
            PHP,
            Python,
            Go,
            CSharp
        }

        private const string CODE_TMP_FILENAME = "code";
        // 临时代码文件路径
        string codeTmpPath = "";
        // 开始执行代码前做的事
        private Action preWork;
        // 执行代码后做的事
        private Action<string> afterWork;

        private string code;
        private ProgramLan selectedLan;

        public CodeHelper SetPreWork(Action work)
        {
            this.preWork = work;
            return this;
        }

        public CodeHelper SetAfterWork(Action<string> work)
        {
            this.afterWork = work;
            return this;
        }


        /// <summary>
        /// 执行代码
        /// </summary>
        public void Run(string code, ProgramLan selectedLan)
        {
            this.code = code;
            this.selectedLan = selectedLan;
            Thread thread = new Thread(ExecuteByProgramLan);
            thread.Start();
        }



        /// <summary>
        /// 通过编程语言执行代码
        /// </summary>
        /// <param name="code"></param>
        private void ExecuteByProgramLan()
        {
            if(preWork != null)
            {
                preWork();
            }
            //// 创建临时文件
            codeTmpPath = Constants.APP_STARTUP_PATH + CODE_TMP_FILENAME;
            switch (selectedLan)
            {
                case ProgramLan.JavaScript:
                    codeTmpPath += ".js";
                    break;
                case ProgramLan.PHP:
                    codeTmpPath += ".php";
                    break;
                case ProgramLan.Python:
                    codeTmpPath += ".py";
                    break;
                case ProgramLan.Go:
                    codeTmpPath += ".go";
                    break;
                default:
                    codeTmpPath += ".tmp";
                    break;
            }
            FileStream fs = new FileStream(codeTmpPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            // end 创建临时文件

            Func<string> runCode = null;
            switch (selectedLan)
            {
                case ProgramLan.JavaScript:
                    sw.Write(code);
                    runCode = RunNode;
                    break;
                case ProgramLan.PHP:
                    runCode = RunPHP;
                    sw.WriteLine("<?php ");
                    sw.Write(code);
                    break;
                case ProgramLan.Python:
                    runCode = RunPython;
                    sw.Write(code);
                    break;
                case ProgramLan.Go:
                    sw.Write(code);
                    runCode = RunGo;
                    break;
                default:
                    sw.Write(code);
                    runCode = DefaultRunCode;
                    break;
            }
            sw.Close(); // 关闭输入流
            Task<String> task = new Task<String>(runCode);
            task.Start();
            task.Wait();
            if(afterWork != null)
            {
                afterWork(task.Result);
            }
            // 删除临时文件
            if (File.Exists(codeTmpPath))
            {
                File.Delete(codeTmpPath);
            }
        }

        /// <summary>
        /// 公共ProcessInfo
        /// </summary>
        /// <returns></returns>
        private ProcessStartInfo GetCommonProcessInfo()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            return info;
        }

        /// <summary>
        /// 转化字符串编码
        /// </summary>
        /// <returns></returns>
        private string GetCorrectString(string errStr)
        {
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(errStr));
        }


        /// <summary>
        /// 运行js脚本
        /// </summary>
        /// <returns></returns>
        private string RunNode()
        {
            Process scriptProc = new Process();
            ProcessStartInfo info = GetCommonProcessInfo();
            info.Arguments = codeTmpPath;
            info.FileName = "node.exe";
            scriptProc.StartInfo = info;
            try
            {
                scriptProc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("node未安装或者未设置node环境变量！");
                return "";
            }
            string outStr = GetCorrectString(scriptProc.StandardOutput.ReadToEnd());
            // 有错误，读取错误信息
            if (String.IsNullOrEmpty(outStr))
            {
                outStr = GetCorrectString(scriptProc.StandardError.ReadToEnd());
            }
            scriptProc.Close();
            return outStr;

        }

        /// <summary>
        /// 运行PHP脚本
        /// </summary>
        /// <returns></returns>
        private string RunPHP()
        {
            Process scriptProc = new Process();
            ProcessStartInfo info = GetCommonProcessInfo();
            info.FileName = Constants.APP_STARTUP_PATH + Common.Settings.PHPDirName.Value
                + "/php.exe";
            info.Arguments = "-f " + codeTmpPath;
            scriptProc.StartInfo = info;
            try
            {
                scriptProc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("PHP目录不存在！");
                return "";
            }
            string outStr = GetCorrectString(scriptProc.StandardOutput.ReadToEnd());
            // 有错误，读取错误信息
            if (String.IsNullOrEmpty(outStr))
            {
                outStr = GetCorrectString(scriptProc.StandardError.ReadToEnd());
            }
            scriptProc.Close();
            return outStr;

        }

        /// <summary>
        /// 运行Python脚本
        /// </summary>
        /// <returns></returns>
        private string RunPython()
        {
            Process scriptProc = new Process();
            ProcessStartInfo info = GetCommonProcessInfo();
            info.FileName = "python.exe";
            info.Arguments = codeTmpPath;
            scriptProc.StartInfo = info;
            try
            {
                scriptProc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Python环境未安装或未添加到环境变量！");
                return "";
            }
            string outStr = GetCorrectString(scriptProc.StandardOutput.ReadToEnd());
            // 有错误，读取错误信息
            if (String.IsNullOrEmpty(outStr))
            {
                outStr = GetCorrectString(scriptProc.StandardError.ReadToEnd());
            }
            scriptProc.Close();
            return outStr;

        }

        /// <summary>
        /// 运行Go脚本
        /// </summary>
        /// <returns></returns>
        public string RunGo()
        {
            Process scriptProc = new Process();
            ProcessStartInfo info = GetCommonProcessInfo();
            info.FileName = "go.exe";
            info.Arguments = "run " + codeTmpPath;
            scriptProc.StartInfo = info;
            try
            {
                scriptProc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Go环境未安装！");
                return "";
            }
            string outStr = GetCorrectString(scriptProc.StandardOutput.ReadToEnd());
            // 有错误，读取错误信息
            if (String.IsNullOrEmpty(outStr))
            {
                outStr = GetCorrectString(scriptProc.StandardError.ReadToEnd());
            }
            scriptProc.Close();
            return outStr;
        }

        /// <summary>
        /// 默认执行方式
        /// </summary>
        /// <returns></returns>
        public string DefaultRunCode()
        {
            return "还未实现哦 \\﻿(•◡•)/";
        }

    }
}
