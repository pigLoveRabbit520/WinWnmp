using SalamanderWnmp.Configuration;
using SalamanderWnmp.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SalamanderWnmp.Tool.RedisHelper;

namespace SalamanderWnmp
{
    class Common
    {
        private static Ini settings = null;
        /// <summary>
        /// 应用程序配置(单例模式)
        /// </summary>
        public static Ini Settings {
            get
            {
                if(settings == null)
                {
                    settings = new Ini();
                }
                return settings;
            }
        }

        /// <summary>
        /// 不能实例化Common类
        /// </summary>
        private Common()
        {

        }


        // 应用启动目录
        public static readonly String APP_STARTUP_PATH = AppDomain.CurrentDomain.BaseDirectory;


        // Redis连接配置列表
        public static Dictionary<string, RedisConnConfig> ConnConfigList = null;



        public static readonly MysqlProgram Mysql = new MysqlProgram();
        public static readonly WnmpProgram Nginx = new NginxProgram();
        public static readonly PHPProgram PHP = new PHPProgram();

    }
}
