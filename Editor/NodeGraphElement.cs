using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MikanLab.NodeGraph
{
    [CustomGraphView(typeof(NodeGraph))]
    public class NodeGraphElement : GraphView
    {
        
        protected List<VisualNode> vnodeList = new();
        public NodeGraph target;
        protected SerializedObject s_target;
        protected SerializedProperty s_nodes;
        public Dictionary<Type, int> nodeCache = new();

        //配置部分
        public NodeGraphElement(NodeGraph target) : base()
        {
            //交互操作
            //AddToClassList("Mikan-graph-view");
            //styleSheets.Add(GUIUtilities.GraphViewColored);
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(0.5f, ContentZoomer.DefaultMaxScale);
            Insert(0, new GridBackground());

            //创建搜索树
            var searchWindowProvider = ScriptableObject.CreateInstance<NodeSeracher>();
            searchWindowProvider.Initialize(this);
            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
            };

            deleteSelection += DeleteNode;
            
        }

        public void Bind(NodeGraph target)
        {
            target.MeetNodeLimit();
            this.target = target;
        }

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in ports.ToList())
            {
                if (startAnchor.node == port.node ||
                    startAnchor.direction == port.direction ||
                    startAnchor.portType != port.portType)
                {
                    continue;
                }

                compatiblePorts.Add(port);
            }
            return compatiblePorts;
        }

        /// <summary>
        /// 从资源中加载
        /// </summary>
        public void LoadFromAsset()
        {
            this.s_target = new(target);
            this.s_nodes = s_target.FindProperty("NodeList");

            for (int i = 0; i < target.NodeList.Count; ++i)
            {
                var node = target.NodeList[i];
                var visualNode = new VisualNode(node);
                visualNode.Bind(s_nodes.GetArrayElementAtIndex(i));

                AddElement(visualNode);
                if (!nodeCache.ContainsKey(visualNode.Type)) nodeCache.Add(visualNode.Type, 0);
                nodeCache[visualNode.Type]++;
                vnodeList.Add(visualNode);

                visualNode.SetPosition(new Rect(node.OnGraphData.Position, new Vector2(100f, 200f)));
            }

            for (int i = 0; i < target.NodeList.Count; ++i)
            {
                foreach (var edges in target.NodeList[i].OutputPorts)
                {
                    Port outputPort = vnodeList[i].outputContainer.Query<Port>().Where(e => e.portName == edges.Key).First();
                    foreach (var edge in edges.Value.Edges)
                    {
                        Port inputPort = vnodeList[edge.TargetIndex].inputContainer.Query<Port>().Where(e => e.portName == edge.TargetPortName).First();


                        if (inputPort != null && outputPort != null)
                        {
                            // 创建边
                            Edge visualEdge = new Edge();

                            // 连接端口
                            visualEdge.input = inputPort;
                            visualEdge.output = outputPort;

                            // 将边添加到 GraphView
                            AddElement(visualEdge);

                            // 更新端口连接状态 (可选，但推荐)
                            inputPort.Connect(visualEdge);
                            outputPort.Connect(visualEdge);
                        }
                        else
                        {
                            throw new("Could not find ports to connect.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 保存更改至资源
        /// </summary>
        public void SaveChangeToAsset()
        {
            SaveSerialized();

            Dictionary<Node, int> indexs = new();
            target.NodeList.Clear();

            int i = 0;
            foreach (var node in nodes)
            {
                var preData = (node as VisualNode).Data;
                preData.OnGraphData.NodeName = node.title;
                preData.OnGraphData.Position = node.GetPosition().position;
                preData.Index = i;
                indexs[node] = i;
                i++;
                foreach(var outport in preData.OutputPorts) outport.Value.Edges.Clear();
                foreach(var inport in preData.InputPorts) inport.Value.Edges.Clear();

                target.NodeList.Add(preData);
                
            }

            foreach (var edge in edges)
            {
                var outdata = new BaseNode.EdgeData();
                var indata = new BaseNode.EdgeData();

                if (!indexs.ContainsKey(edge.input.node) || !indexs.ContainsKey(edge.output.node)) continue;

                outdata.TargetPortName = edge.input.portName;
                outdata.TargetIndex = indexs[edge.input.node];

                indata.TargetPortName = edge.output.portName;
                indata.TargetIndex = indexs[edge.output.node];

                var outnode = target.NodeList[indexs[edge.output.node]];
                var innode = target.NodeList[indexs[edge.input.node]];

                outnode.OutputPorts[edge.output.portName].Edges.Add(outdata);
                innode.InputPorts[edge.input.portName].Edges.Add(indata);
            }
        }

        public virtual void AddNewNode(Type nodeType)
        {
            SaveSerialized();
            
            //添加新的节点
            var newnode = BaseNode.CreateNode(nodeType);
            var node = new VisualNode(newnode);

            AddElement(node);
            vnodeList.Add(node);
         
            SaveChangeToAsset();
            
            DeleteElements(edges);
            DeleteElements(nodes);
            nodeCache.Clear();
            vnodeList.Clear();

            LoadFromAsset();
        }
         
        public virtual void DeleteNode(string op,AskUser askUser)
        {
            var selections = selection.ToList();

            SaveSerialized();
            
            DeleteSelection();

            SaveChangeToAsset();
            
            DeleteElements(edges);
            DeleteElements(nodes);
            nodeCache.Clear();
            vnodeList.Clear();

            LoadFromAsset();
        }

        public virtual void SaveSerialized()
        {
            s_target.Update();
            s_target.ApplyModifiedProperties();
        }

        public virtual void Execute() { }
    }

    /// <summary>
    /// 节点图适用的视图，默认为NodeGraphElement
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomGraphViewAttribute : Attribute
    {
        public Type Type;
        public CustomGraphViewAttribute(Type viewType) 
        {
            this.Type = viewType;
        }
    }
}