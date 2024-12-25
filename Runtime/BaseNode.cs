using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MikanLab.NodeGraph
{
    /// <summary>
    /// 最基本的节点
    /// </summary>
    [Serializable]
    public abstract class BaseNode
    {
        #region 序列化子数据
        /// <summary>
        /// 端口数据
        /// </summary>
        [Serializable]
        public class PortData
        {
            public string PortType;
            public bool AllowMultiple;
            public List<EdgeData> Edges = new();
        }

        /// <summary>
        /// 连接数据
        /// </summary>
        [Serializable]
        public class EdgeData
        {
            public string TargetPortName;
            public int TargetIndex;
        }

        /// <summary>
        /// 显示数据
        /// </summary>
        [Serializable]
        public class DisplayData
        {
            public string NodeName = "";
            public Vector2 Position = new(0, 0);
            public bool Deleteable = true;
        }
        #endregion

        #region 字段

        [NonSerialized] public NodeGraph Owner;
        [SerializeField] public int Index = 0;
        [SerializeField] public PortDictionary InputPorts = new();
        [SerializeField] public PortDictionary OutputPorts = new();
        [SerializeField] public DisplayData OnGraphData = new();

        /// <summary>
        /// 添加输入端口
        /// </summary>
        /// <param name="portType"></param>
        /// <param name="portName"></param>
        /// <param name="ifMultiple"></param>
        public void AddInputPort(Type portType, string portName = "", bool ifMultiple = false)
        {
            if (portName == "") portName = portType.Name;
            foreach (var Port in InputPorts)
            {
                if (Port.Key == portName)
                {
                    Debug.LogError($"端口名称重复：{portName}，操作中止");
                    return;
                }
            }
            InputPorts.Add(portName, new PortData() { PortType = portType.AssemblyQualifiedName, AllowMultiple = ifMultiple });
        }

        /// <summary>
        /// 添加输出端口
        /// </summary>
        /// <param name="portType"></param>
        /// <param name="portName"></param>
        /// <param name="ifMultiple"></param>
        public void AddOutputPort(Type portType, string portName = "", bool ifMultiple = false)
        {
            if (portName == "") portName = portType.Name;
            foreach (var Port in OutputPorts)
            {
                if (Port.Key == portName)
                {
                    Debug.LogError($"端口名称重复：{portName}，操作中止");
                    return;
                }
            }
            OutputPorts.Add(portName, new PortData() { PortType = portType.AssemblyQualifiedName, AllowMultiple = ifMultiple });
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public static BaseNode CreateNode(Type nodeType)
        {
            if (!nodeType.IsSubclassOf(typeof(BaseNode))) return null;
            return Activator.CreateInstance(nodeType) as BaseNode;
        }

        #endregion

        #region 遍历相关
        
        /// <summary>
        /// 获取输出端口的第一个连接的节点
        /// </summary>
        /// <param name="portName">端口名称</param>
        /// <returns></returns>
        public virtual BaseNode GetFirstOutputTo(string portName)
        {
            if(OutputPorts.ContainsKey(portName))
            {
                if (OutputPorts[portName].Edges == null || OutputPorts[portName].Edges.Count == 0) return null;
                else return Owner.NodeList[OutputPorts[portName].Edges[0].TargetIndex];
            }
            return null;
        }

        /// <summary>
        /// 获取输出端口的所有连接节点
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public virtual List<BaseNode> GetOutputTos(string portName)
        {
            List<BaseNode> returnList = new();
            if (OutputPorts.ContainsKey(portName))
            {
                if (OutputPorts[portName].Edges == null || OutputPorts[portName].Edges.Count == 0) return null;
                else
                {
                    foreach(var edge in OutputPorts[portName].Edges)
                    {
                        returnList.Add(Owner.NodeList[edge.TargetIndex]);
                    }
                    return returnList;
                }
            }
            else return null;
        }

        /// <summary>
        /// 在指定端口接收输入
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public virtual object[] GetInput(string portName)
        {
            if (InputPorts.ContainsKey(portName))
            {
                if (InputPorts[portName].Edges == null || InputPorts[portName].Edges.Count == 0) return null;
                else return Owner.NodeList[InputPorts[portName].Edges[0].TargetIndex].SendOutput(InputPorts[portName].Edges[0].TargetPortName);
            }
            return null;
        }

        /// <summary>
        /// 在指定端口发送输出
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public virtual object[] SendOutput(string portName) 
        {
            return null;
        }
        #endregion
    }
}
