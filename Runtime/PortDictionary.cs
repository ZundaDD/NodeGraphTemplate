using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikanLab.NodeGraph
{
    /// <summary>
    /// 序列化字典
    /// </summary>
    [Serializable]
    public class PortDictionary : Dictionary<string, BaseNode.PortData>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<string> keys;
        [SerializeField] private List<BaseNode.PortData> values;
        
        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
            keys.Clear();
            keys = null;
            values.Clear();
            values = null;
        }

        public void OnBeforeSerialize()
        {
            keys = new();
            values = new();
            foreach (var keypair in this)
            {
                keys.Add(keypair.Key);
                values.Add(keypair.Value);
            }
        }
    }
}