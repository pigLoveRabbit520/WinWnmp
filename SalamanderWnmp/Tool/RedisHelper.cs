using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;

namespace SalamanderWnmp.Tool
{
    /// <summary>
    /// Redis面板辅助类
    /// </summary>
    class RedisHelper
    {
        // redis连接配置文件
        private static readonly string CONN_FILE = Constants.APP_STARTUP_PATH + "redis_conns.dat";

        public enum NodeType
        {
            Connnection = 2,
            Database = 3,
            Key = 4
        }

        /// <summary>
        /// Redis连接配置
        /// </summary>
        [Serializable]
        public class RedisConnConfig
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


            /// <summary>
            /// 获取连接字符串
            /// </summary>
            /// <returns></returns>
            public string GetConnectionStr()
            {
                return Host + ":" + Port + ",allowAdmin=true";
            }

            /// <summary>
            /// 主机加端口字符串
            /// </summary>
            /// <returns></returns>
            public string GetHostAndPortStr()
            {
                return Host + ":" + Port;
            }
        }

        /// <summary>
        /// 节点类
        /// </summary>s
        public class Node: INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public void Notify(string propertyName)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private string name;
            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    this.name = value;
                    this.Notify("Name");
                }
            }

            /// <summary>
            /// 节点类型
            /// </summary>
            public NodeType NodeType { get; set; }


            private ObservableCollection<Node> nodes;
            /// <summary>
            /// 子节点指针
            /// </summary>
            public ObservableCollection<Node> Nodes
            {
                get
                {
                    return this.nodes;
                }
                set
                {
                    this.nodes = value;
                    this.Notify("Nodes");
                }
            }

        }

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

        /// <summary>
        /// 生成顶级Node
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<Node> BuildNodes(Dictionary<string, RedisConnConfig> ConnConfigList)
        {
            ObservableCollection<Node> nodes = new ObservableCollection<Node>(); 
            foreach (var item in ConnConfigList)
            {
                nodes.Add(new Node {
                    Name = item.Key,
                    NodeType = NodeType.Connnection,
                    Nodes = new ObservableCollection<Node>()
                });
            }
            return nodes;
        }
    }
}
