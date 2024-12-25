using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
using UnityEditor.PackageManager.UI;

namespace MikanLab.NodeGraph
{
    [CustomGraphWindow(typeof(NodeGraph))]
    public class NodeGraphWindow : EditorWindow
    {
        protected virtual string prefKey => "MikanLab_Node_Graph_Window";
        protected virtual Type prefType => typeof(Setting);

        protected bool ifInited = false;
        protected bool ifFirst = true;
        protected NodeGraph target;
        protected Setting setting;
        protected Toolbar toolbar;
        protected NodeGraphView graph;

        #region 偏好设置
        [Serializable]
        public class Setting
        {
            public float width = 500f;
            public float height = 300f;
            public float position_x = 0f;
            public float position_y = 0f;
        }
        #endregion

        #region 生命周期
        public static void Invoke(NodeGraph target)
        {
            var window = GetWindow<NodeGraphWindow>("NodeGraphWindow");
            if (!window.ifInited)
            {   
                window.target = target;
                window.SetLayout(window.prefKey, window.prefType);
            }
        }

        protected virtual void OnEnable()
        {
            if (!ifInited && !ifFirst) SetLayout(prefKey, prefType);
        }

        protected virtual void OnDestroy()
        {
            SavePref(prefKey);
            SaveGraph();
        }

        protected virtual void OnDisable()
        {
            ifInited = false;
            SavePref(prefKey);
            SaveGraph();
        }
        #endregion

        #region 绘制布局
        protected virtual void SetLayout(string prefkey,Type prefType)
        {
            setting = ReadPref(prefkey, prefType);
            SetFromPref();
            AddElements();
            ifInited = true;
            ifFirst = false;
        }

        protected virtual void AddElements()
        {
            toolbar = new();
            toolbar.style.flexDirection = FlexDirection.Row;
            
            rootVisualElement.Add(toolbar);

            graph = Activator.CreateInstance(EditorUtilities.GetGraphView(target.GetType())) as NodeGraphView;
            graph.Bind(target);
            graph.style.flexGrow = 1;
            graph.LoadFromAsset();
            rootVisualElement.Add(graph);

            //工具栏
            toolbar.Add(new ToolbarButton(graph.Execute) { text = "测试" });

        }

        protected virtual void SetFromPref()
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

        protected virtual void SavePref(string keyname)
        {
            setting.width = position.width;
            setting.height = position.height;
            setting.position_x = position.x;
            setting.position_y = position.y;
            EditorPrefs.SetString(keyname, JsonUtility.ToJson(setting));
        }

        protected virtual Setting ReadPref(string keyname,Type settingType)
        {
            if (!EditorPrefs.HasKey(keyname)) return Activator.CreateInstance(settingType) as Setting;
            else return JsonUtility.FromJson(EditorPrefs.GetString(keyname), settingType) as Setting;
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