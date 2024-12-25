using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace MikanLab.NodeGraph
{
    public class VisualNode : Node
    {
        public SerializedProperty serializedProperty;
        public BaseNode Data;
        public Type Type => Data.GetType();

        public VisualNode(BaseNode data)
        {
            Data = data;
            title = Data.OnGraphData.NodeName;
            if (!Data.OnGraphData.Deleteable) capabilities -= Capabilities.Deletable;
            foreach (var inputPort in data.InputPorts) AddInputPort(inputPort.Key,inputPort.Value);
            foreach (var outputPort in data.OutputPorts) AddOutputPort(outputPort.Key,outputPort.Value);
            
        }

        public void Bind(SerializedProperty serializedProperty)
        {
            this.serializedProperty = serializedProperty;
            DrawNode();
        }

        public void AddInputPort(string name,BaseNode.PortData pd)
        {
            var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, pd.AllowMultiple ? Port.Capacity.Multi : Port.Capacity.Single, Type.GetType(pd.PortType));
            inputPort.portName = name;
            inputContainer.Add(inputPort);
        }

        public void AddOutputPort(string name,BaseNode.PortData pd)
        {
            var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, pd.AllowMultiple ? Port.Capacity.Multi : Port.Capacity.Single, Type.GetType(pd.PortType));
            outputPort.portName = name;
            outputContainer.Add(outputPort);
        }

        /// <summary>
        /// 绘制节点
        /// </summary>
        public void DrawNode()
        {
            var drawerType = EditorUtilities.GetNodeDrawers(Data.GetType());
            var newDrawer = Activator.CreateInstance(drawerType) as NodeElementDrawer;
            newDrawer.Bind(serializedProperty, this);
            newDrawer.OnDrawer();

            RefreshExpandedState();
        }
    }
}
