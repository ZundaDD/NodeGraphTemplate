using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MikanLab.NodeGraph
{
    public static class EditorUtilities
    {
        #region 对应类型查询
        private static Dictionary<Type, Type> nodeDrawers;
        private static Dictionary<Type, Type> graphViews;
        private static Dictionary<Type, Type> graphWindows;

        public static Type GetNodeDrawers(Type type)
        {
            if (!type.IsSubclassOf(typeof(BaseNode))) throw new Exception("Invalid Query!");
            if (nodeDrawers == null) InitDict();
            while (!nodeDrawers.ContainsKey(type))
            {
                if(type.BaseType != null) type = type.BaseType;
                else
                {
                    type = typeof(BaseNode);
                    break;
                }
            }
            return nodeDrawers[type];
        }

        public static Type GetGraphView(Type type)
        {
            if (!type.IsAssignableTo(typeof(NodeGraph))) throw new Exception("Invalid Query!");
            if (graphViews == null) InitDict();
            while (!graphViews.ContainsKey(type))
            {
                if (type.BaseType != null) type = type.BaseType;
                else
                {
                    type = typeof(NodeGraph);
                    break;
                } 
            }
            return graphViews[type];
        }

        public static Type GetGrpahWindow(Type type)
        {
            if (!type.IsAssignableTo(typeof(NodeGraph))) throw new Exception("Invalid Query!");
            if (graphWindows == null) InitDict();
            while (!graphWindows.ContainsKey(type))
            {
                if (type.BaseType != null) type = type.BaseType;
                else
                {
                    type = typeof(NodeGraph);
                    break;
                }
            }
            return graphWindows[type];
        }

        /// <summary>
        /// 能否将当前类实例赋值给目标类引用
        /// </summary>
        /// <param name="origin">当前类</param>
        /// <param name="target">目标类</param>
        /// <returns></returns>
        public static bool IsAssignableTo(this Type origin, Type target)
        {
            return target.IsAssignableFrom(origin);
        }

        
        #endregion

        #region 节点类型获取
        private static List<Type> allNodes;
        private static Dictionary<Type, List<Type>> graphNodes;
        private static Dictionary<Type, Dictionary<Type, uint>> nodeMax;

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

        /// <summary>
        /// 寻找节点图可添加的节点
        /// </summary>
        /// <param name="type">节点图类型</param>
        private static void SearchNode(System.Type type)
        {
            if (allNodes == null) InitDict();
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

        #region 反射
        private static void InitDict()
        {
            nodeDrawers = new();
            graphWindows = new();
            graphViews = new();
            allNodes = new();
            nodeDrawers.Add(typeof(BaseNode), typeof(NodeDrawer));
            graphWindows.Add(typeof(NodeGraph), typeof(NodeGraphWindow));
            graphViews.Add(typeof(NodeGraph), typeof(NodeGraphView));

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    //处理视图
                    if (type.IsSubclassOf(typeof(NodeGraphView)))
                    {
                        var attr = type.GetCustomAttributes(typeof(CustomGraphViewAttribute), false) as CustomGraphViewAttribute[];
                        if (attr == null || attr.Length == 0) continue;
                        else
                        {
                            graphViews.Add(attr[0].Type, type);
                        }
                    }
                    //处理绘制器
                    else if (type.IsSubclassOf(typeof(NodeDrawer)))
                    {
                        var attr = type.GetCustomAttributes(typeof(CustomNodeDrawerAttribute), false) as CustomNodeDrawerAttribute[];
                        if (attr == null || attr.Length == 0) continue;
                        else
                        {
                            nodeDrawers.Add(attr[0].Type, type);
                        }
                    }
                    //处理窗口
                    else if (type.IsSubclassOf(typeof(NodeGraphWindow)))
                    {
                        var attr = type.GetCustomAttributes(typeof(CustomGraphWindowAttribute), false) as CustomGraphWindowAttribute[];
                        if (attr == null || attr.Length == 0) continue;
                        else
                        {
                            graphWindows.Add(attr[0].Type, type);
                        }
                    }
                    else if (type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(BaseNode))))
                    {
                        allNodes.Add(type);
                    }
                }
            }
        }
        #endregion

        #region 处理资源打开
        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenGraph(int instanceID,int linenumber)
        {
            string path = AssetDatabase.GetAssetPath(instanceID);
            Type type = AssetDatabase.GetMainAssetTypeAtPath(path);

            if(type == typeof(NodeGraph) || type.IsSubclassOf(typeof(NodeGraph)))
            {
                var windowtype = EditorUtilities.GetGrpahWindow(type);
                windowtype.InvokeMember("Invoke", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null,
                    new object[] { AssetDatabase.LoadAssetAtPath<NodeGraph>(path) });
                return true;
            }
            return false;
        }
        #endregion
    }
}