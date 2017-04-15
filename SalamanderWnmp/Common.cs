using SalamanderWnmp.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        // 应用启动目录
        public static readonly String APP_STARTUP_PATH = AppDomain.CurrentDomain.BaseDirectory;


        // Redis连接配置列表
        public static Dictionary<string, RedisConnConfig> ConnConfigList = null;

    }
}
