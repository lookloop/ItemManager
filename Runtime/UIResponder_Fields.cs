using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class UIResponder
{
    /// <summary>背包模板数组 — 每项生成一个独立容器</summary>
    public BackpackTemplate[] templates;

    public bool autoBuild = true;
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public Camera uiCamera;
    [HideInInspector] public string PointerDownTag;
}
}
