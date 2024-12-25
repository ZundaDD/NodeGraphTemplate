using UnityEditor;
using UnityEngine;

namespace MikanLab.NodeGraph
{
    [CustomPropertyDrawer(typeof(BaseNode.EdgeData))]
    public class EdgeDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty portNameProperty = property.FindPropertyRelative("TargetPortName");
            SerializedProperty targetIndexProperty = property.FindPropertyRelative("TargetIndex");

            position.height = EditorGUIUtility.singleLineHeight;
            
            EditorGUI.PropertyField(position, portNameProperty,new GUIContent("连接端口"));
            position.y += EditorGUI.GetPropertyHeight(portNameProperty, GUIContent.none);
            position.y += EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(position, targetIndexProperty, new GUIContent("节点索引"));
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //  获取子属性
            SerializedProperty portNameProperty = property.FindPropertyRelative("TargetPortName");
            SerializedProperty targetIndexProperty = property.FindPropertyRelative("TargetIndex");

            // 计算总高度
            float totalHeight = 0;
            totalHeight += EditorGUI.GetPropertyHeight(portNameProperty, GUIContent.none);
            totalHeight += EditorGUI.GetPropertyHeight(targetIndexProperty, GUIContent.none);
            totalHeight += EditorGUIUtility.standardVerticalSpacing;

            return totalHeight;
        }
    }
}
