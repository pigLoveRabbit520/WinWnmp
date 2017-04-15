using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalamanderWnmp.Configuration
{
    /// <summary>
    /// Redis连接配置
    /// </summary>
    [Serializable]
    class RedisConnConfig
    {
        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnName { get; set; }

        /// <summary>
        /// 主机名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 验证
        /// </summary>
        public string Auth { get; set; }
    }
}
