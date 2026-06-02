using UnityEngine;

namespace Lookloop.ItemManager
{
/// <summary>
/// Item 数据管理器 — 操控 items 数组 + 顺手同步视图。
/// 外部赋值 items → 自动带动 Cell 视觉刷新。
///
/// ─── 5 个公开方法 ───
///   构建数据    → 按 totalItems/cellCount 创建 items[] + 同步首页
///   设置Cell    → 写入 items[index]，若在当前可见页则同步 UI
///   同步页面    → 整页刷新
///   下一页      → 翻页 + 刷新
///   上一页      → 翻页 + 刷新
/// </summary>
public static class ItemDataManager
{
    // ════════════════════════════════════════════════════════════
    // 核心：设置格子 — 数据写入 + 视觉同步
    // ════════════════════════════════════════════════════════════
    public static void SetCell(UIResponder _this, int index, Item item)
    {
        if (ContainerManager.containers != null && ContainerManager.containers.Count > 0)
            ContainerManager.containers[0].items[index] = item;
        ItemView.Sync(_this, index);
    }

    // ════════════════════════════════════════════════════════════
    // 构建数据 — 创建 items[] + 同步首页
    // ════════════════════════════════════════════════════════════
    public static void BuildData(UIResponder _this)
    {
        var t = ContainerManager.containers[0].template;
        // Prefab 模式：items 长度 = Cell 数量；动态模式：取 totalItems 和 cellCount 较大值
        int dataSize = (t != null && t.prefab != null)
            ? ItemTouch.cellCount
            : Mathf.Max(t != null ? t.totalItems : 20, ItemTouch.cellCount);
        ContainerManager.containers[0].items = new Item[dataSize];
        SyncPage(_this);
    }

    // ════════════════════════════════════════════════════════════
    // 设置Cell — 外部入口：写全量数组，若在当前页则刷新视图
    // ════════════════════════════════════════════════════════════
    public static void SetCellData(UIResponder _this, int index, Item item)
    {
        if (index < 0 || index >= ContainerManager.containers[0].items.Length) return;
        ContainerManager.containers[0].items[index] = item;

        int pageSize  = ItemTouch.cellCount;
        int pageStart = ContainerManager.containers[0].currentPage * pageSize;
        if (index >= pageStart && index < pageStart + pageSize)
            SetCell(_this, index - pageStart, item);
    }

    // ════════════════════════════════════════════════════════════
    // 翻页
    // ════════════════════════════════════════════════════════════
    public static void NextPage(UIResponder _this)
    {
        int maxPage = Mathf.Max(0, (ContainerManager.containers[0].items.Length - 1) / ItemTouch.cellCount);
        if (ContainerManager.containers[0].currentPage < maxPage) ContainerManager.containers[0].currentPage++;
        SyncPage(_this);
    }

    public static void PrevPage(UIResponder _this)
    {
        if (ContainerManager.containers[0].currentPage > 0) ContainerManager.containers[0].currentPage--;
        SyncPage(_this);
    }

    // ════════════════════════════════════════════════════════════
    // 同步页面 — 整页刷新
    // ════════════════════════════════════════════════════════════
    public static void SyncPage(UIResponder _this)
    {
        int pageSize  = ItemTouch.cellCount;
        int pageStart = ContainerManager.containers[0].currentPage * pageSize;

        for (int i = 0; i < ItemTouch.cellCount; i++)
        {
            int dataIndex = pageStart + i;
            if (dataIndex < ContainerManager.containers[0].items.Length)
                SetCell(_this, i, ContainerManager.containers[0].items[dataIndex]);
            else
                SetCell(_this, i, null);
        }
    }
}
}