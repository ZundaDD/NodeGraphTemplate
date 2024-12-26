# Node Graph Template
这是一个根据Unity的GraphView组件而创建的一个节点图资源模板

## 如何使用
为了实现您自己的图类型，您只需要定义节点就可以做到，如果要对图的功能做出拓展，可能涉及到对NodeGraph，NodeGraphView
NodeGraphWindow进行拓展。直接继承这些类就可以获取其所有职能，不要忘记定义[CustomGraphView]和[CustomGraphWindow]
来指定视图和窗口应用的图对象。</br>

---
定义节点的流程很简单，只需要在构造函数中设定好节点名称、端口信息，然后定义相关属性：[UsedFor]用于指出能使用该节点的图类型，
[UniversalUsed]使节点在任何图中被检测到，然后可以通过重写Execute方式来补充该节点的遍历法则。
```
[Serializable]
[UsedFor(typeof(NodeGraph))]
public class Input : BaseNode
{
    public Input() 
    {
        OnGraphData.NodeName = "Input";
        AddOutputPort(typeof(int), "Link", true);
    }
}
```
---
同时，如果您想让节点有额外的绘制方式，如同Inspector中那样，可以定制NodeDrawer来实现。以下是一个样例：
```
[CustomNodeDrawer(typeof(Item))]
class ItemDrawer : NodeDrawer
{
    public override void OnDrawer()
    {
        base.OnDrawer();

        var itemproperty = property.FindPropertyRelative("item");
        PropertyField text = new();
        text.BindProperty(itemproperty);
        text.styleSheets.Add(GUIUtilities.PropertyFieldLessenLabel);

        visualNode.extensionContainer.Add(text);
    }
}
```

---
如果需要对窗口进行拓展，可以通过Create->MikanLab->NodeGraph->WindosTemplate中创建脚本模板，其中提供了较为详细
的拓展引导。
## 其他
如果您在使用过程中发现问题或者觉得有可以改进的地方，可以在Issues板块中提出。</br>

## 引用资源
本项目引用的一切图标均来源于[FlatIcon](https://www.flaticon.com/ "免费图标素材")</br>