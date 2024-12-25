using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;

namespace MikanLab.NodeGraph
{
    public class NodeSeracher : ScriptableObject, ISearchWindowProvider
    {
        private NodeGraphElement graphView;


        public void Initialize(NodeGraphElement graphView)
        {
            this.graphView = graphView;
        }

        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {

            var entries = new List<SearchTreeEntry>();
            var limitDict = GraphUtilities.GetGraphLimit(graphView.target.GetType());

            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));

            foreach (var nodeType in GraphUtilities.GetGraphValideNode(graphView.target.GetType()))
            {
                //上限检测
                if (limitDict.ContainsKey(nodeType))
                {
                    uint maxLimit = limitDict[nodeType];
                    uint curCount = 0;
                    if(graphView.nodeCache.ContainsKey(nodeType))
                    {
                        curCount = (uint)graphView.nodeCache[nodeType];
                    }
                    
                    if (maxLimit != 0 && curCount >= maxLimit) continue;
                }

                entries.Add(new SearchTreeEntry(new GUIContent(nodeType.Name)) { level = 1, userData = nodeType });
            }

            return entries;
        }

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as System.Type;
            graphView.AddNewNode(type);
            return true;
        }
    }
}