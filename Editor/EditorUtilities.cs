using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikanLab.NodeGraph
{
    public static class EditorUtilities
    {
        private static Dictionary<Type, Type> nodeDrawers;

        public static Type GetNodeDrawers(Type type)
        {
            if (!type.IsSubclassOf(typeof(BaseNode))) throw new Exception("Invalid Query!");
            if (nodeDrawers == null) InitDrawerDict();
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

        private static void InitDrawerDict()
        {
            nodeDrawers = new();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttributes(typeof(GraphDrawerAttribute), false) as GraphDrawerAttribute[];
                    if (attr == null || attr.Length == 0) continue;
                    else
                    {
                        nodeDrawers.Add(attr[0].Type , type);
                    }
                }
            }
        }
    }
}