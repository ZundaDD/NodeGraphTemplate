using System;
using System.Collections.Generic;
using UnityEngine;

namespace MikanLab.NodeGraph
{
    /// <summary>
    /// 存储类反射相关信息
    /// </summary>
    public static class GraphUtilities
    {
        private static List<Type> allNodes;
        private static Dictionary<System.Type, List<Type>> graphNodes;
        private static Dictionary<System.Type, Dictionary<System.Type, uint>> nodeMax;

        #region 公开部分
        /// <summary>
        /// 获取节点图可添加的节点
        /// </summary>
        /// <param name="type">节点图类型</param>
        /// <returns></returns>
        public static List<Type> GetGraphValideNode(System.Type type)
        {
            if (graphNodes == null) graphNodes = new();
            if (!graphNodes.ContainsKey(type))
            {
                SearchNode(type);
            }
            return graphNodes[type];
        }

        /// <summary>
        /// 获取节点图的节点个数限制
        /// </summary>
        /// <param name="type">节点图类型</param>
        /// <returns></returns>
        public static Dictionary<System.Type, uint> GetGraphLimit(System.Type type)
        {
            if (nodeMax == null) nodeMax = new();
            if (!nodeMax.ContainsKey(type))
            {
                SearchLimit(type);
            }
            return nodeMax[type];

        }
        #endregion

        #region 反射
        /// <summary>
        /// 寻找节点图可添加的节点
        /// </summary>
        /// <param name="type">节点图类型</param>
        private static void SearchNode(System.Type type)
        {
            if (allNodes == null) SearchAllNode();
            graphNodes[type] = new();
            foreach (var node in allNodes)
            {
                if (node.IsDefined(typeof(UniversalUsedAttribute), true))
                {
                    graphNodes[type].Add(node);
                    continue;
                }
                foreach (var attr in node.GetCustomAttributes(typeof(UsedForAttribute), true))
                {
                    var attrs = attr as UsedForAttribute;
                    if (!attrs.AllowInherit && attrs.GraphType != type) continue;
                    if (attrs.AllowInherit && type != attrs.GraphType && !type.IsSubclassOf(attrs.GraphType)) continue;
                    graphNodes[type].Add(node);
                }
            }
        }

        /// <summary>
        /// 寻找所有节点
        /// </summary>
        private static void SearchAllNode()
        {
            allNodes = new();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(BaseNode))))
                    {
                        allNodes.Add(type);
                    }
                }
            }
        }

        private static void SearchLimit(System.Type type)
        {
            if (nodeMax == null) nodeMax = new();
            nodeMax[type] = new();
            foreach (var attr in type.GetCustomAttributes(typeof(CountLimitAttribute), true))
            {
                var attrs = attr as CountLimitAttribute;
                nodeMax[type][attrs.NodeType] = attrs.Max;
            }
        }
        #endregion
    }
}