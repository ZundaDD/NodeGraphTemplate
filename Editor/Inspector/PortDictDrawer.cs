using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MikanLab.NodeGraph
{
    [CustomPropertyDrawer(typeof(PortDictionary))]
    public class PortDictDrawer : PropertyDrawer
    {
        bool fold = false;
        bool[] element;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            fold = EditorGUI.Foldout(position,fold, label);
            if (fold)
            {
                EditorGUI.indentLevel++;

                position.y += EditorGUIUtility.singleLineHeight;
                position.y += EditorGUIUtility.standardVerticalSpacing;

                var keys = property.FindPropertyRelative("keys");
                var values = property.FindPropertyRelative("values");

                if(element == null) element = new bool[keys.arraySize];
                
                for (int i = 0; i < keys.arraySize; ++i)
                {

                    var key = keys.GetArrayElementAtIndex(i);
                    var value = values.GetArrayElementAtIndex(i);

                    element[i] = EditorGUI.Foldout(position,element[i], new GUIContent(key.stringValue));
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    if (element[i])
                    {
                        EditorGUI.indentLevel++;

                        EditorGUI.PropertyField(position, value);
                        position.y += EditorGUI.GetPropertyHeight(value);
                        position.y += EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.indentLevel--;
                    }

                }

                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var values = property.FindPropertyRelative("values");

            float totalHeight = 0f;
            
            totalHeight += EditorGUIUtility.singleLineHeight;
            if (fold)
            {
                totalHeight +=
                    (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight)
                    * (values.arraySize);

                for (int i = 0; i < values.arraySize; ++i)
                {
                    if (element[i])
                    {
                        var value = values.GetArrayElementAtIndex(i);
                        totalHeight += EditorGUI.GetPropertyHeight(value);
                        totalHeight += EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
            return totalHeight;
        }
    }
}
