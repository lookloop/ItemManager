using UnityEngine;

namespace Lookloop.ItemManager
{
/// <summary>
/// Item 数据管理器 — 操控 ContainerMod.items 数组。
/// </summary>
public static class ItemDataManager
{
    /// <summary>
    /// 写入物品数据。containerId=容器索引，index=物品槽位。
    /// 方法内直接 new Item() 完成赋值。
    /// </summary>
    public static void SetItem(int containerId, int index, int id, int type, int tier, int count, int[] data)
    {
        var containers = ContainerManager.containers;
        if (containers == null || containerId < 0 || containerId >= containers.Count)
            return;

        var mod = containers[containerId];
        if (mod.items == null || index < 0 || index >= mod.items.Length)
            return;

        var item = new Item(id, type, tier, count, data);
        mod.items[index] = item;
    }

    public static void SetItem(int containerId, int index, Item item)
    {
        var containers = ContainerManager.containers;
        if (containers == null || containerId < 0 || containerId >= containers.Count)
            return;

        var mod = containers[containerId];
        if (mod.items == null || index < 0 || index >= mod.items.Length)
            return;

        mod.items[index] = item;
    }
}
}
