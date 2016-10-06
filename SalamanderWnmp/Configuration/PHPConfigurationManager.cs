using System;
using System.IO;
using System.Windows;

namespace SalamanderWnmp.Configuration
{
    class PHPConfigurationManager
    {
        [Flags]
        public enum PHPExtension
        {
            Disabled = 1,
            Enabled  = 2,
            ZendExt  = 3,
        }
        public bool[] UserPHPExtentionValues;
        public string[] phpExtName;
        public PHPExtension[] phpExtensions;

        private string ExtensionPath;
        private string IniFilePath;
        private string TmpIniFile;

        private void LoadPHPIni()
        {
            TmpIniFile = File.ReadAllText(IniFilePath);
        }

        public void LoadPHPExtensions(string phpDirName)
        {

            ExtensionPath = UI.MainWindow.StartupPath + String.Format("/{0}/ext/", phpDirName);
            IniFilePath = UI.MainWindow.StartupPath + String.Format("/{0}/php.ini", phpDirName);
            if (!Directory.Exists(ExtensionPath))
            {
                MessageBox.Show("php 扩展目录未找到");
                return;
            }
            phpExtName = Directory.GetFiles(ExtensionPath, "*.dll");
            phpExtensions = new PHPExtension[phpExtName.Length];
            UserPHPExtentionValues = new bool[phpExtName.Length];

            for (var i = 0; i < phpExtName.Length; i++) {
                phpExtName[i] = phpExtName[i].Remove(0, ExtensionPath.Length);
            }

            LoadPHPIni();
            ParsePHPIni();
        }

        public void ParsePHPIni()
        {
            using (var sr = new StringReader(TmpIniFile)) {
                string str;
                while ((str = sr.ReadLine()) != null) {
                    for (var i = 0; i < phpExtName.Length; i++) {
                        if (str.StartsWith(";extension=" + phpExtName[i])) {
                            phpExtensions[i] = PHPExtension.Disabled;
                            break;
                        }
                        if (str.StartsWith("extension=" + phpExtName[i])) {
                            phpExtensions[i] = PHPExtension.Enabled;
                            break;
                        }
                        if (str.StartsWith(";zend_extension=" + phpExtName[i])) {
                            phpExtensions[i] = PHPExtension.Disabled | PHPExtension.ZendExt;
                            break;
                        }
                        if (str.StartsWith("zend_extension=" + phpExtName[i])) {
                            phpExtensions[i] = PHPExtension.Enabled | PHPExtension.ZendExt;
                            break;
                        }
                    }
                }
            }
        }

        public void SavePHPIniOptions()
        {
            for (var i = 0; i < phpExtName.Length; i++) {
                if (!phpExtensions[i].HasFlag(PHPExtension.ZendExt)) {
                    if (UserPHPExtentionValues[i]) {
                        if (phpExtensions[i].HasFlag(PHPExtension.Disabled))
                            TmpIniFile = TmpIniFile.Replace(";extension=" + phpExtName[i], "extension=" + phpExtName[i]);
                    } else {
                        if (phpExtensions[i].HasFlag(PHPExtension.Enabled))
                            TmpIniFile = TmpIniFile.Replace("extension=" + phpExtName[i], ";extension=" + phpExtName[i]);
                    }
                } else { // Special case zend_extension
                    if (UserPHPExtentionValues[i]) {
                        if (phpExtensions[i].HasFlag(PHPExtension.Disabled))
                            TmpIniFile = TmpIniFile.Replace(";zend_extension=" + phpExtName[i], "zend_extension=" + phpExtName[i]);
                    } else {
                        if (phpExtensions[i].HasFlag(PHPExtension.Enabled))
                            TmpIniFile = TmpIniFile.Replace("zend_extension=" + phpExtName[i], ";zend_extension=" + phpExtName[i]);
                    }
                }
            }
            File.WriteAllText(IniFilePath, TmpIniFile);
        }
    }
}
