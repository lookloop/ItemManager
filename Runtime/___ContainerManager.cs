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
    public static List<ContainerData> containers;

    // ════════════════════════════════════════════════════════════
    // 注册 — 构建好的 GameObject → 分配 ID → 加入 containers 列表
    // ════════════════════════════════════════════════════════════
    public static int Register(GameObject containerObj, UIResponder _this)
    {
        if (containers == null)
            containers = new List<ContainerData>();

        var cd = new ContainerData
        {
            container = containerObj.transform as RectTransform,
            items     = new Item[ItemTouch.cellCount]
        };

        containers.Add(cd);

        return containers.Count - 1;
    }

    // ════════════════════════════════════════════════════════════
    // 注销 — 从列表移除，可选销毁 GameObject
    // ════════════════════════════════════════════════════════════
    public static void Unregister(UIResponder _this, int id, bool destroyGO = true)
    {
        if (containers == null || id < 0 || id >= containers.Count) return;

        var cd = containers[id];
        if (destroyGO && cd.container != null)
            Object.Destroy(cd.container.gameObject);

        containers.RemoveAt(id);
    }

    // ════════════════════════════════════════════════════════════
    // 显示 / 隐藏
    // ════════════════════════════════════════════════════════════
    public static void Show(UIResponder _this, int id)
    {
        if (containers == null || id < 0 || id >= containers.Count) return;
        var cd = containers[id];
        if (cd.container != null)
            cd.container.gameObject.SetActive(true);
    }

    public static void Hide(UIResponder _this, int id)
    {
        if (containers == null || id < 0 || id >= containers.Count) return;
        var cd = containers[id];
        if (cd.container != null)
            cd.container.gameObject.SetActive(false);
    }

    // ════════════════════════════════════════════════════════════
    // 移动容器到指定位置
    // ════════════════════════════════════════════════════════════
    public static void MoveTo(UIResponder _this, int id, Vector2 anchoredPosition)
    {
        if (containers == null || id < 0 || id >= containers.Count) return;
        var cd = containers[id];
        if (cd.container != null)
            cd.container.anchoredPosition = anchoredPosition;
    }
}
}