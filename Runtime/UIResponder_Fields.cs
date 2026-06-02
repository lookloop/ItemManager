using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class UIResponder
{
    /// <summary>容器蓝图数组 — 每项生成一个独立容器</summary>
    public ContainerSpec[] mods;

    [HideInInspector] public Canvas canvas;
    [HideInInspector] public string PointerDownTag;
}
}
