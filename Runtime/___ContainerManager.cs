using UnityEngine;

namespace Lookloop.ItemManager
{
/// <summary>
/// Container 操控器 — 注册/注销容器、显示/隐藏容器、移动容器位置。
/// 不涉及 UI 构建，不管格子数据。
/// </summary>
public static class ContainerManager
{
    // ════════════════════════════════════════════════════════════
    // 注册 — 构建好的 GameObject → 分配 ID → 加入 containers 列表
    // ════════════════════════════════════════════════════════════
    public static int 注册(GameObject containerObj, UIResponder _this)
    {
        if (_this.containers == null)
            _this.containers = new System.Collections.Generic.List<ContainerData>();

        var cd = new ContainerData
        {
            container = containerObj.transform as RectTransform,
            items     = new Item[_this.cellCount]
        };

        _this.containers.Add(cd);
        _this.items = cd.items; // 向后兼容快捷引用

        return _this.containers.Count - 1;
    }

    // ════════════════════════════════════════════════════════════
    // 注销 — 从列表移除，可选销毁 GameObject
    // ════════════════════════════════════════════════════════════
    public static void 注销容器(UIResponder _this, int id, bool destroyGO = true)
    {
        if (_this.containers == null || id < 0 || id >= _this.containers.Count) return;

        var cd = _this.containers[id];
        if (destroyGO && cd.container != null)
            Object.Destroy(cd.container.gameObject);

        _this.containers.RemoveAt(id);
    }

    // ════════════════════════════════════════════════════════════
    // 显示 / 隐藏
    // ════════════════════════════════════════════════════════════
    public static void 显示容器(UIResponder _this, int id)
    {
        if (_this.containers == null || id < 0 || id >= _this.containers.Count) return;
        var cd = _this.containers[id];
        if (cd.container != null)
            cd.container.gameObject.SetActive(true);
    }

    public static void 隐藏容器(UIResponder _this, int id)
    {
        if (_this.containers == null || id < 0 || id >= _this.containers.Count) return;
        var cd = _this.containers[id];
        if (cd.container != null)
            cd.container.gameObject.SetActive(false);
    }

    // ════════════════════════════════════════════════════════════
    // 移动容器到指定位置
    // ════════════════════════════════════════════════════════════
    public static void 移动容器到(UIResponder _this, int id, Vector2 anchoredPosition)
    {
        if (_this.containers == null || id < 0 || id >= _this.containers.Count) return;
        var cd = _this.containers[id];
        if (cd.container != null)
            cd.container.anchoredPosition = anchoredPosition;
    }
}
}
