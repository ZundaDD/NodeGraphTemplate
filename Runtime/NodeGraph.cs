using MikanLab;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MikanLab.NodeGraph
{
    [CreateAssetMenu(fileName = "NodeGraph",menuName = "MikanLab/NodeGraph")]
    [Serializable]
    /// <summary>
    /// 最基本的节点图
    /// </summary>
    public class NodeGraph : ScriptableObject
    {
        #region 序列化
        /// <summary>
        /// 节点列表
        /// </summary>
        [SerializeReference] public List<BaseNode> NodeList = new();
        #endregion

        /// <summary>
        /// 满足节点最少数量需求
        /// </summary>
        public virtual void MeetNodeLimit()
        {
            foreach (var attr in GetType().GetCustomAttributes(typeof(CountLimitAttribute), true))
            {
                uint i = 0, tarMin = (attr as CountLimitAttribute).Min;
                string tarName = (attr as CountLimitAttribute).NodeType.AssemblyQualifiedName;
                Type tarType = (attr as CountLimitAttribute).NodeType;


                foreach (var node in NodeList)
                {
                    if (node.GetType().AssemblyQualifiedName == tarName) i++;
                }
                if (i < tarMin)
                {
                    var node = BaseNode.CreateNode(tarType);
                    node.OnGraphData.Position = new((tarMin - i) * 100f, 0f);
                    NodeList.Add(node);
                    i++;
                }
            }
        }

        public void OnEnable()
        {
            foreach (var node in NodeList) node.Owner = this;    
        }

        public virtual void Execute() { }

        /// <summary>
        /// 运行时加载图
        /// </summary>
        /// <param name="path">相对Resources文件夹的路径</param>
        /// <returns>图对象</returns>
        public static NodeGraph RuntimeLoad(string path)
        {
            var graph = Resources.Load(path) as NodeGraph;
            if (graph == null) throw new Exception("Type doen't match!\nNodeGraph required!");
            foreach (var node in graph.NodeList) node.OnGraphData = null;
            return graph;
        }
    }
}