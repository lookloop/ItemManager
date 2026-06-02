using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class UIResponder
{
    /// <summary>背包模板数组 — 每项生成一个独立容器</summary>
    public ContainerMod[] mods;

    [HideInInspector] public Canvas canvas;
    [HideInInspector] public string PointerDownTag;
}
}
