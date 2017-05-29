using SalamanderWnmp.Tool;
using SalamanderWnmp.UserClass;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SalamanderWnmp.Tool.RedisHelper;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// RedisWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RedisWin : SalamanderWindow
    {
        // Connection Map
        private static Dictionary<string, ConnectionMultiplexer> redisConns = 
            new Dictionary<string, ConnectionMultiplexer>();
        private ObservableCollection<Node> nodes = null;

        public RedisWin()
        {
            InitializeComponent();
            if(Common.ConnConfigList == null)
            {
                Common.ConnConfigList = RedisHelper.GetConnList();
            }
            this.nodes = RedisHelper.BuildNodes(Common.ConnConfigList);
            this.tvConn.ItemsSource = this.nodes;
        }


        private void btnAddConnect_Click(object sender, RoutedEventArgs e)
        {
            AddRedisConnWin win = new AddRedisConnWin();
            win.Owner = this;
            win.ShowType = 0;
            win.Closing += AddRedisConnWin_Closing;
            win.ShowDialog();
            e.Handled = true;
        }

        private void AddRedisConnWin_Closing(object sender, CancelEventArgs e)
        {
            AddRedisConnWin win = sender as AddRedisConnWin;
            if(win.Tag != null && win.Tag.GetType().Name == "RedisConnConfig")
            {
                RedisConnConfig config = win.Tag as RedisConnConfig;
                if(win.ShowType == 0)
                {
                    Common.ConnConfigList.Add(config.ConnName, config);
                    nodes.Add(new Node { Name = config.ConnName, NodeType = NodeType.Connnection });
                }
                else
                {
                    Common.ConnConfigList[config.ConnName] = config;
                    (tvConn.SelectedItem as Node).Name = config.ConnName;
                }
                RedisHelper.WriteConnList(Common.ConnConfigList);
            }
        }

        private void TreeViewItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Node node = tvConn.SelectedItem as Node;
            if(node.NodeType == NodeType.Connnection)
            {
                if (!redisConns.ContainsKey(node.Name))
                {
                    try
                    {
                        RedisConnConfig config = node.Value as RedisConnConfig;
                        ConnectionMultiplexer conn = 
                            ConnectionMultiplexer.Connect(config.GetConnectionStr());
                        redisConns.Add(node.Name, conn);
                        IServer server = conn.GetServer(config.GetHostAndPortStr());
                        KeyValuePair<string, string>[] kvs = server.ConfigGet("databases");
                        int databasesNum = int.Parse(kvs[0].Value);
                        for (int i = 0; i < databasesNum; i++)
                        {
                            node.Nodes.Add(new Node
                            {
                                Name = "db" + i,
                                NodeType = NodeType.Database,
                                Value = i,
                                Nodes = new ObservableCollection<Node>()
                            });
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Connect failed!");
                    }
                }
            }
            else if (node.NodeType == NodeType.Database)
            {
                int dbNum = (int)node.Value;
                Node nodeParent = GetConnectionNode(node);
                ConnectionMultiplexer conn = redisConns[nodeParent.Name];
                RedisConnConfig config = nodeParent.Value as RedisConnConfig;
                IServer server = conn.GetServer(config.GetHostAndPortStr());
                // show all keys
                IEnumerable<RedisKey> keysNew = server.Keys(dbNum, pattern: "*");
                if(node.Nodes.Count > 0)
                {
                    foreach (var keyNew in keysNew)
                    {
                        if (IsKeyNameExists(keyNew, node.Nodes))
                            continue;
                        else
                        {
                            node.Nodes.Add(new Node
                            {
                                Name = keyNew.ToString(),
                                NodeType = NodeType.Key,
                                Value = keyNew,
                                Nodes = new ObservableCollection<Node>()
                            });
                        }
                    }
                }
                else
                {
                    foreach (var key in keysNew)
                    {
                        node.Nodes.Add(new Node
                        {
                            Name = key.ToString(),
                            NodeType = NodeType.Key,
                            Value = key,
                            Nodes = new ObservableCollection<Node>()
                        });
                    }
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// key名称是否已经存在
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private bool IsKeyNameExists(string keyName ,ObservableCollection<Node> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Name == keyName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取Connection node实例
        /// </summary>
        /// <param name="nodeDb"></param>
        /// <returns></returns>
        private Node GetConnectionNode(Node nodeDb)
        {
            foreach (var node in nodes)
            {
                if (node.Nodes.Contains(nodeDb))
                    return node;
            }
            return null;
        }

        private Node GetParentNode(int index)
        {
            try
            {
                if (tvConn.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                {
                    return null;
                }
                TreeViewItem itemParent = (tvConn.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItem).Parent as TreeViewItem;
                return itemParent.DataContext as Node;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    
}
