using UnityEngine;
using TMPro;

namespace Lookloop.ItemManager
{
public partial class Core
{
    public TMP_FontAsset font;

    public ContainerSpec[] specs;

    [HideInInspector] public Canvas canvas;

    /// <summary>与 specs 一一对应的运行时容器数组，用 spec 下标索引</summary>
    [HideInInspector] public Container[] containers;
}
}
