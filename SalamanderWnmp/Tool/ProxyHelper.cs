using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalamanderWnmp.Tool
{
    class ProxyHelper
    {

        /// <summary>
        /// 获取正在使用的代理
        /// </summary>
        /// <returns></returns>
        private static string GetProxyServer()
        {
            //打开注册表 
            RegistryKey regKey = Registry.CurrentUser;
            string SubKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
            RegistryKey optionKey = regKey.OpenSubKey(SubKeyPath, true);             // 更改健值，设置代理， 
            string actualProxy = optionKey.GetValue("ProxyServer").ToString();
            regKey.Close();
            return actualProxy;
        }

        private static bool GetProxyStatus()
        {
            //打开注册表 
            RegistryKey regKey = Registry.CurrentUser;
            string SubKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
            RegistryKey optionKey = regKey.OpenSubKey(SubKeyPath, true);             // 更改健值，设置代理， 
            int actualProxyStatus = Convert.ToInt32(optionKey.GetValue("ProxyEnable"));
            regKey.Close();
            return actualProxyStatus == 1 ? true : false;
        }
    }
}
