using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class TemplateGenerator
{
    public static string WindowTemplate = "WindowTemplate.txt";

    [MenuItem("Assets/Create/MikanLab/NodeGraph/WindowTemplate", false)]
    public static void CreateWindowTemplate()
    {
        string[] guids = AssetDatabase.FindAssets("WindowTemplate.cs");
        if (guids.Length == 0)
        {
            Debug.LogWarning("WindowTemplate.cs.txt not found in asset database");
            return;
        }
        string templatePath = AssetDatabase.GUIDToAssetPath(guids[0]);

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<CodeGenerator>(),
            "WindowTemplate.cs",
            EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
            templatePath);

    }

    public class CodeGenerator : UnityEditor.ProjectWindowCallback.EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Object file = CreateFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(file);
        }
    }

    /// <summary>
    /// 从模板中创建
    /// </summary>
    /// <param name="pathName"></param>
    /// <param name="resourceFile"></param>
    /// <returns></returns>
    public static Object CreateFromTemplate(string pathName, string resourceFile)
    {
        string fileName = Path.GetFileNameWithoutExtension(pathName).Replace(" ", string.Empty);

        UTF8Encoding encoding = new UTF8Encoding(true, false);

        string template = string.Empty;
        using (StreamReader sr = new(resourceFile))
        {
            template = sr.ReadToEnd();
        }

        template = template.Replace("#FILENAME#", fileName);
        template = template.Replace("#NOTRIM#", string.Empty);

        using (StreamWriter writer = new(Path.GetFullPath(pathName), false, encoding))
        {
            writer.Write(template);
        }
                
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));

    }
}