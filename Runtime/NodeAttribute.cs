using System;

namespace MikanLab.NodeGraph
{
    /// <summary>
    /// 表示该节点会被每一种节点图搜索树检测
    /// </summary>
    public class UniversalUsedAttribute : Attribute
    { }

    /// <summary>
    /// 表示该节点会被哪种节点图搜索树检测到
    /// </summary>
    public class UsedForAttribute : Attribute
    {
        public Type GraphType;
        public bool AllowInherit;
        public UsedForAttribute(Type graphType, bool allowInherit = true)
        {
            GraphType = graphType;
            AllowInherit = allowInherit;
        }
    }

    /// <summary>
    /// 节点的数量限制
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CountLimitAttribute : Attribute
    {
        public Type NodeType;
        public uint Max = 10000;
        public uint Min = 0;
    }

    
}