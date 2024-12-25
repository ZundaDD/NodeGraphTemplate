using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MikanLab.NodeGraph
{
    [GraphDrawer(typeof(BaseNode))]
    public class NodeElementDrawer
    {
        public SerializedProperty property;
        protected VisualNode visualNode;
        
        public void Bind(SerializedProperty nodeProperty, VisualNode visualnode)
        {
            this.visualNode = visualnode;
            property = nodeProperty;
        }
        public virtual void OnDrawer()
        {
            visualNode.extensionContainer.style.backgroundColor = new Color(0x7a / 256f,0x9f / 256f,0xaa / 256f);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false,Inherited = false)]
    public class GraphDrawerAttribute : Attribute
    {
        public Type Type;
        public GraphDrawerAttribute(Type type) => this.Type = type;
    }
}