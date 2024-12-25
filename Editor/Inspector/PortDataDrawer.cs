using UnityEditor;
using UnityEngine;

namespace MikanLab.NodeGraph
{
    [CustomPropertyDrawer(typeof(BaseNode.PortData))]
    public class PortDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            SerializedProperty portType = property.FindPropertyRelative("PortType");
            SerializedProperty allowMultiple = property.FindPropertyRelative("AllowMultiple");
            SerializedProperty edges = property.FindPropertyRelative("Edges");

            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(position, portType, new GUIContent("类型"));
            position.y += EditorGUI.GetPropertyHeight(portType);
            position.y += EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(position, allowMultiple, new GUIContent("多重连接"));
            position.y += EditorGUI.GetPropertyHeight(allowMultiple);
            position.y += EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(position, edges, new GUIContent("连接"));
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty portType = property.FindPropertyRelative("PortType");
            SerializedProperty allowMultiple = property.FindPropertyRelative("AllowMultiple");
            SerializedProperty edges = property.FindPropertyRelative("Edges");

            // 计算总高度
            float totalHeight = 0;
            totalHeight += EditorGUI.GetPropertyHeight(portType, GUIContent.none);
            totalHeight += EditorGUI.GetPropertyHeight(allowMultiple, GUIContent.none);
            totalHeight += EditorGUI.GetPropertyHeight(edges, GUIContent.none);
            totalHeight += EditorGUIUtility.standardVerticalSpacing * 2;
            
            return totalHeight;
        }
    }
}
