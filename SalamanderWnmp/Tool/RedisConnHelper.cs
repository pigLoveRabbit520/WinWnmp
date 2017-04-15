using SalamanderWnmp.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;

namespace SalamanderWnmp.Tool
{
    /// <summary>
    /// Redis连接配置读取设置辅助类
    /// </summary>
    class RedisConnHelper
    {
        // redis连接配置文件
        private static readonly string CONN_FILE = Constants.APP_STARTUP_PATH + "redis_conns.dat";



        /// <summary>
        /// 获取连接配置列表
        /// </summary>
        /// <returns>List<RedisConnConfig></returns>
        /// 
        public static Dictionary<string, RedisConnConfig> GetConnList()
        {
            // 避免处理空流问题
            if (!File.Exists(CONN_FILE))
                return new Dictionary<string, RedisConnConfig>();
            FileStream fs = new FileStream(CONN_FILE, FileMode.Open);
            Dictionary<string, RedisConnConfig> conns = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                conns = (Dictionary<string, RedisConnConfig>)formatter.Deserialize(fs);
                if (conns == null)
                    conns = new Dictionary<string, RedisConnConfig>();
                return conns;
            }
            catch (SerializationException ex)
            {
                MessageBox.Show("Failed to deserialize. Reason: " + ex.Message);
                return new Dictionary<string, RedisConnConfig>();
            }
            finally
            {
                fs.Close();
            }
        }


        /// <summary>
        /// 写入配置列表（序列化）
        /// </summary>
        /// <param name="conns"></param>
        public static void WriteConnList(Dictionary<string, RedisConnConfig> conns)
        {
            FileStream fs = new FileStream(CONN_FILE, FileMode.OpenOrCreate);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, conns);
            }
            catch (SerializationException ex)
            {
                Console.WriteLine("Failed to serialize. Reason: " + ex.Message);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
