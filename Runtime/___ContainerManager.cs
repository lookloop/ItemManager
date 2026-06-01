using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
/// <summary>
/// Container 数据管理器 — 注册容器、增删改查格子数据。
/// 不涉及 UI 构建，只管数据 ↔ 视觉同步。
/// </summary>
public static class ContainerManager
{
    /// <summary>
    /// 注册容器对象 → 分配 ID，初始化 items 数组，写入 containers 列表。
    /// 返回容器 ID（索引）。
    /// </summary>
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

    /// <summary>写入数据 → 触发视觉同步</summary>
    public static void 设置格子(UIResponder _this, int index, Item item)
    {
        if (_this.containers != null && _this.containers.Count > 0)
            _this.containers[0].items[index] = item;
        _this.items[index] = item;
        ___Item视图.同步(_this, index);
    }
}
}
