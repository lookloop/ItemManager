using UnityEngine;
using System.Collections.Generic;

namespace Lookloop.ItemManager
{
/// <summary>
/// Container 操控器 — 注册/注销容器、显示/隐藏容器、移动容器位置。
/// 不涉及 UI 构建，不管格子数据。
/// </summary>
public static class ContainerManager
{
    /// <summary>所有容器数据列表</summary>
    public static List<ContainerMod> containers;

    // ════════════════════════════════════════════════════════════
    // 注册 — 构建好的 GameObject + 模板 → 加入 containers 列表
    // ════════════════════════════════════════════════════════════
    public static int Register(GameObject containerObj, ContainerSpec blueprint)
    {
        if (containers == null)
            containers = new List<ContainerMod>();

        var cd = new ContainerMod
        {
            container   = containerObj.transform as RectTransform,
            items       = new Item[ItemTouch.cellCount],
            blueprint   = blueprint
        };

        containers.Add(cd);

        return containers.Count - 1;
    }
}
}