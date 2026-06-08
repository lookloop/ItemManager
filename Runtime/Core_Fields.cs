using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Lookloop.ItemManager
{
public partial class Core
{
    /// <summary>容器蓝图数组 — 每项生成一个独立容器</summary>
    public TMP_FontAsset font;

    public ContainerSpec[] specs;

    [HideInInspector] public Canvas canvas;

    public List<ContainerMod> containers = new List<ContainerMod>();
}
}
