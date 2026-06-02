using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Lookloop.ItemManager
{
/// <summary>
/// Item 触控总成 — 黑盒处理所有 Item 交互。
/// Core 只做路由：Item → 调这 3 个方法；Container → 自理。
///
/// ─── 3 个核心字段 ───
///   source       来源 cell
///   target       目标 cell
///   itemDragging 跟随手指的 Item
///
/// ─── 3 个公开方法（距离判定拖拽，不再依赖 BeginDrag）───
///   开始点击  → 记录 source + 起手坐标，启计时器
///   拖拽中    → 距离判定 → isDrag；分流：长按跟随 / 滚Grid
///   结算      → isLongPress + isDrag 组合：交换/复位/详情
/// </summary>
public static class ItemTouch
{
}
}