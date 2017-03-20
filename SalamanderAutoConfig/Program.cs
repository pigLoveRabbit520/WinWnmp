using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SalamanderAutoConfig
{
    class Program
    {
        private readonly static string APP_STARTUP_PATH = AppDomain.CurrentDomain.BaseDirectory;

        private readonly static string nginxConfFile = APP_STARTUP_PATH + "nginx/conf/nginx.conf";

        private readonly static string phpConfFile = APP_STARTUP_PATH + "php/php.ini";

        private readonly static string mysqlConfFile = APP_STARTUP_PATH + "mysql/my.ini";

        static void Main(string[] args)
        {
            ConfigNginx();
            ConfigPHP();
            ConfigMysql();
            Console.ReadKey();
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
                File.WriteAllText(nginxConfFile, reg.Replace(txt, "root " + APP_STARTUP_PATH.Replace("\\", "/") + "nginx/html;", 1));
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
                    "extension_dir = \"" + APP_STARTUP_PATH + "php\\ext\"", 1);
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
                txt = reg1.Replace(txt, "basedir=" + APP_STARTUP_PATH.Replace("\\", "/") 
                    + "mysql", 1);
                // 第二个匹配
                Regex reg2 = new Regex(@"datadir=\*+/data");
                txt = reg2.Replace(txt, "datadir=" + APP_STARTUP_PATH.Replace("\\", "/") 
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
