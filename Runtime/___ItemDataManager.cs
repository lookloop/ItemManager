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
        
    }
}