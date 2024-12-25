using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;

namespace MikanLab.NodeGraph
{
    [CustomGraphWindow(typeof(NodeGraph))]
    public class GraphWindow : EditorWindow
    {
        private static readonly string prefKey = "MikanLab_Node_Graph_Window";
        private bool ifInited = false;
        private bool ifFirst = true;
        protected NodeGraph target;
        protected Setting setting;
        protected Toolbar toolbar;
        protected NodeGraphElement graph;

        #region 偏好设置
        [Serializable]
        public class Setting
        {
            public float width = 500f;
            public float height = 300f;
            public float position_x = 0f;
            public float position_y = 0f;
            public bool propertyWindowOn = false;
        }
        #endregion

        #region 生命周期
        public static void Invoke(NodeGraph target)
        {
            var window = GetWindow<GraphWindow>("NodeGraphWindow");
            if (!window.ifInited)
            {
                window.target = target;

                if (!EditorPrefs.HasKey(prefKey)) window.setting = new();
                else window.setting = JsonUtility.FromJson<Setting>(EditorPrefs.GetString(prefKey));
                window.FromPref();

                window.AddElements();
                window.ifInited = true;
                window.ifFirst = false;
            }
        }

        protected virtual void OnEnable()
        {
            if (!ifInited && !ifFirst)
            {
                if (!EditorPrefs.HasKey(prefKey)) setting = new();
                else setting = JsonUtility.FromJson<Setting>(EditorPrefs.GetString(prefKey));
                FromPref();
                AddElements();
                ifInited = true;
                ifFirst = false;
            }
        }

        protected virtual void OnDestroy()
        {
            SavePref();
            SaveGraph();
        }

        protected virtual void OnDisable()
        {
            ifInited = false;
            SavePref();
            SaveGraph();
        }
        #endregion


        #region 绘制控制
        protected virtual void AddElements()
        {
            toolbar = new();
            toolbar.style.flexDirection = FlexDirection.Row;
            

            rootVisualElement.Add(toolbar);

            graph = Activator.CreateInstance( EditorUtilities.GetGraphView(target.GetType())) as NodeGraphElement;
            graph.Bind(target);
            graph.style.flexGrow = 1;
            graph.LoadFromAsset();
            rootVisualElement.Add(graph);

            //工具栏
            toolbar.Add(new ToolbarButton(graph.Execute) { text = "测试" });

        }

        protected virtual void FromPref()
        {
            position = new(setting.position_x, setting.position_y, setting.width, setting.height);
        }
        #endregion

        #region 保存
        private void SaveGraph()
        {
            if (target == null) return;
            if (graph == null) return;

            graph.SaveChangeToAsset();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
        }

        private void SavePref()
        {
            setting.width = position.width;
            setting.height = position.height;
            setting.position_x = position.x;
            setting.position_y = position.y;
            EditorPrefs.SetString(prefKey, JsonUtility.ToJson(setting));
        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomGraphWindowAttribute : Attribute
    {
        public Type Type;
        public CustomGraphWindowAttribute(Type type) => this.Type = type;
    }
}