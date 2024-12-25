using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;

namespace MikanLab.NodeGraph
{
    public class GraphWindow<TGraph,TView> : EditorWindow where TGraph : NodeGraph where TView : NodeGraphElement, new()
    {
        private static readonly string prefKey = "MikanLab" + typeof(TGraph).Name + typeof(TView).Name;
        private bool ifInited = false;
        private bool ifFirst = true;
        private TGraph target;
        private Setting setting;
        private Toolbar toolbar;

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
        public static void Invoke(TGraph target)
        {
            var window = GetWindow<GraphWindow<TGraph,TView>>("NodeGraphWindow");
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

        private void OnEnable()
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

        private void OnDestroy()
        {
            SavePref();
            SaveGraph();
        }

        private void OnDisable()
        {
            ifInited = false;
            SavePref();
            SaveGraph();
        }
        #endregion

        #region 元素组件
        private NodeGraphElement graph;
        private NodeGraphElement Graph
        {
            get
            {
                if (graph == null)
                {
                    graph = new TView() { style = { flexGrow = 1 } };
                    graph.Bind(target);
                    graph.LoadFromAsset();
                }
                return graph;
            }

        }

        #endregion

        #region 绘制控制
        private void AddElements()
        {
            toolbar = new();
            toolbar.style.flexDirection = FlexDirection.Row;
            rootVisualElement.Add(toolbar);

            rootVisualElement.Add(Graph);

            //工具栏
            toolbar.Add(new ToolbarButton(Graph.Execute) { text = "测试" });

        }

        private void FromPref()
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
}