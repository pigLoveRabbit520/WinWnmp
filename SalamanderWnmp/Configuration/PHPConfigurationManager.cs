using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace SalamanderWnmp.Configuration
{
    /// <summary>
    /// 不考虑zend扩展
    /// </summary>
    class PHPConfigurationManager
    {
        [Flags]
        public enum PHPExtensionFlag
        {
            Disabled = 1,
            Enabled  = 2,
            ZendExt  = 3,
        }
        public bool[] UserPHPExtentionValues;
        public string[] phpExtNames;
        public PHPExtensionFlag[] flags;
        // 集合对象
        private List<ExtesionHelper> extesions = new List<ExtesionHelper>();

        private string ExtensionPath;
        private string IniFilePath;
        private string TmpIniFile;

        private void LoadPHPIni()
        {
            TmpIniFile = File.ReadAllText(IniFilePath);
        }

        /// <summary>
        /// 获取ext目录下所有扩展
        /// </summary>
        /// <param name="phpDirName">php目录名</param>
        public void LoadPHPExtensions(string phpDirName)
        {

            ExtensionPath = Constants.APP_STARTUP_PATH + String.Format("{0}/ext/", phpDirName);
            IniFilePath = Constants.APP_STARTUP_PATH + String.Format("{0}/php.ini", phpDirName);
            if (!Directory.Exists(ExtensionPath))
            {
                MessageBox.Show("php 扩展目录未找到");
                return;
            }
            phpExtNames = Directory.GetFiles(ExtensionPath, "*.dll");
            flags = new PHPExtensionFlag[phpExtNames.Length];
            UserPHPExtentionValues = new bool[phpExtNames.Length];

            for (var i = 0; i < phpExtNames.Length; i++) {
                string name = phpExtNames[i].Remove(0, ExtensionPath.Length);
                phpExtNames[i] = name;
                extesions.Add(new ExtesionHelper() { ExtName = name });
            }

            LoadPHPIni();
            ParsePHPIni();
        }

        public void ParsePHPIni()
        {
            using (var sr = new StringReader(TmpIniFile)) {
                string str;
                while ((str = sr.ReadLine()) != null) {
                    for (var i = 0; i < phpExtNames.Length; i++) {
                        if (str.StartsWith(";extension=" + phpExtNames[i])) {
                            flags[i] = PHPExtensionFlag.Disabled;
                            extesions[i].IsLoad = false;
                            break;
                        }
                        if (str.StartsWith("extension=" + phpExtNames[i])) {
                            flags[i] = PHPExtensionFlag.Enabled;
                            extesions[i].IsLoad = true;
                            break;
                        }
                        if (str.StartsWith(";zend_extension=" + phpExtNames[i])) {
                            flags[i] = PHPExtensionFlag.Disabled | PHPExtensionFlag.ZendExt;
                            break;
                        }
                        if (str.StartsWith("zend_extension=" + phpExtNames[i])) {
                            flags[i] = PHPExtensionFlag.Enabled | PHPExtensionFlag.ZendExt;
                            break;
                        }
                    }
                }
            }
        }

        public void SavePHPIniOptions()
        {
            for (var i = 0; i < extesions.Count; i++) {
                if(extesions[i].IsLoad)
                {
                    TmpIniFile = Regex.Replace(TmpIniFile, "[;]+extension=" + phpExtNames[i], "extension=" + phpExtNames[i]);
                }
                else
                {
                    // 已经注释了
                    if (Regex.IsMatch(TmpIniFile, "[ ]*[;]+[ ]*extension=" + phpExtNames[i]))
                        continue;
                    else
                    {
                        TmpIniFile = Regex.Replace(TmpIniFile, "[ ]*extension=" + phpExtNames[i], ";extension=" + phpExtNames[i]);
                    }
                }
            }

            File.WriteAllText(IniFilePath, TmpIniFile);

        }

        /// <summary>
        /// 扩展状态对象集合
        /// </summary>
        /// <returns></returns>
        public List<ExtesionHelper> GetExtensions()
        {
            return this.extesions;
        }

        public class ExtesionHelper
        {
            /// <summary>
            /// 扩展名
            /// </summary>
            public string ExtName { get; set; }

            /// <summary>
            /// 是否加载
            /// </summary>
            public bool IsLoad { get; set; }
        }
    }
}
