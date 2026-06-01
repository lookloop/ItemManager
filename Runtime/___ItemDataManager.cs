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
    public static void 设置格子(UIResponder _this, int index, Item item)
    {
        if (_this.containers != null && _this.containers.Count > 0)
            _this.containers[0].items[index] = item;
        _this.items[index] = item;
        ___Item视图.同步(_this, index);
    }

    // ════════════════════════════════════════════════════════════
    // 构建数据 — 创建 items[] + 同步首页
    // ════════════════════════════════════════════════════════════
    public static void 构建数据(UIResponder _this)
    {
        int dataSize = Mathf.Max(_this.totalItems, _this.cellCount);
        _this.items = new Item[dataSize];
        同步页面(_this);
    }

    // ════════════════════════════════════════════════════════════
    // 设置Cell — 外部入口：写全量数组，若在当前页则刷新视图
    // ════════════════════════════════════════════════════════════
    public static void 设置Cell(UIResponder _this, int index, Item item)
    {
        if (index < 0 || index >= _this.items.Length) return;
        _this.items[index] = item;

        int pageSize  = _this.cellCount;
        int pageStart = _this.currentPage * pageSize;
        if (index >= pageStart && index < pageStart + pageSize)
            设置格子(_this, index - pageStart, item);
    }

    // ════════════════════════════════════════════════════════════
    // 翻页
    // ════════════════════════════════════════════════════════════
    public static void 下一页(UIResponder _this)
    {
        int maxPage = Mathf.Max(0, (_this.items.Length - 1) / _this.cellCount);
        if (_this.currentPage < maxPage) _this.currentPage++;
        同步页面(_this);
    }

    public static void 上一页(UIResponder _this)
    {
        if (_this.currentPage > 0) _this.currentPage--;
        同步页面(_this);
    }

    // ════════════════════════════════════════════════════════════
    // 同步页面 — 整页刷新
    // ════════════════════════════════════════════════════════════
    public static void 同步页面(UIResponder _this)
    {
        int pageSize  = _this.cellCount;
        int pageStart = _this.currentPage * pageSize;

        for (int i = 0; i < _this.cellCount; i++)
        {
            int dataIndex = pageStart + i;
            if (dataIndex < _this.items.Length)
                设置格子(_this, i, _this.items[dataIndex]);
            else
                设置格子(_this, i, null);
        }
    }
}
}