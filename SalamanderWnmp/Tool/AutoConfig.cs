using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SalamanderWnmp.Tool
{
    class AutoConfig
    {
        private readonly static string nginxConfFile = Constants.APP_STARTUP_PATH + "nginx/conf/nginx.conf";

        private readonly static string phpConfFile = Constants.APP_STARTUP_PATH + "php/php.ini";

        private readonly static string mysqlConfFile = Constants.APP_STARTUP_PATH + "mysql/my.ini";

        /// <summary>
        /// 自动配置
        /// </summary>
        public static void Run()
        {
            ConfigNginx();
            ConfigPHP();
            ConfigMysql();
        }

        /// <summary>
        /// 配置Nginx
        /// </summary>
        private static void ConfigNginx()
        {
            if (File.Exists(nginxConfFile))
            {
                string txt = File.ReadAllText(nginxConfFile);
                Regex reg = new Regex(@"root D:/\*+;");
                File.WriteAllText(nginxConfFile, reg.Replace(txt, "root " + Constants.APP_STARTUP_PATH.Replace("\\", "/") + "nginx/html;", 1));
                Console.WriteLine("-----------------Nginx配置完成-----------------");
            }
            else
            {
                Console.WriteLine("Nginx配置文件没找到");
            }
        }

        /// <summary>
        /// 配置PHP
        /// </summary>
        private static void ConfigPHP()
        {
            if (File.Exists(phpConfFile))
            {
                string txt = File.ReadAllText(phpConfFile);
                Regex reg = new Regex("extension_dir = \"your_path" + @"\\" + "ext\"");
                string res = reg.Replace(txt,
                    "extension_dir = \"" + Constants.APP_STARTUP_PATH + "php\\ext\"", 1);
                File.WriteAllText(phpConfFile, res);
                Console.WriteLine("-----------------PHP配置完成-----------------");
            }
            else
            {
                Console.WriteLine("php配置文件没找到");
            }
        }

        /// <summary>
        /// 配置Mysql
        /// </summary>
        private static void ConfigMysql()
        {
            if (File.Exists(mysqlConfFile))
            {
                string txt = File.ReadAllText(mysqlConfFile);
                Regex reg1 = new Regex(@"basedir=\*+");
                txt = reg1.Replace(txt, "basedir=" + Constants.APP_STARTUP_PATH.Replace("\\", "/")
                    + "mysql", 1);
                // 第二个匹配
                Regex reg2 = new Regex(@"datadir=\*+/data");
                txt = reg2.Replace(txt, "datadir=" + Constants.APP_STARTUP_PATH.Replace("\\", "/")
                    + @"mysql/data", 1);
                File.WriteAllText(mysqlConfFile, txt);
                Console.WriteLine("-----------------Mysql配置完成-----------------");
            }
            else
            {
                Console.WriteLine("mysql配置文件没找到");
            }
        }
    }
}
