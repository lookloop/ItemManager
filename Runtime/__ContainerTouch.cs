using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
/// <summary>
/// Container 触控总成 — 面板拖拽。
/// UIResponder 只做路由：Container → 调这 3 个方法。
///
/// ─── 字段 ───
///   source  被拖拽的面板
///
/// ─── 3 个公开方法 ───
///   开始拖拽  → 记录起始状态
///   拖拽中    → 面板跟随手指
///   结算      → 清理
/// </summary>
public static class ContainerTouch
{
    public static GameObject source;
    static Vector2 beginPosition;
    static Vector2 dragStartPos;

    // ════════════════════════════════════════════════════════════
    // 1. 开始拖拽 — A 阶段
    // ════════════════════════════════════════════════════════════
    public static void BeginDrag(UIResponder _this, PointerEventData eventData)
    {
        source        = eventData.pointerCurrentRaycast.gameObject;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _this.canvas.transform as RectTransform, eventData.position, _this.uiCamera, out beginPosition);
        dragStartPos  = (source.transform as RectTransform).anchoredPosition;
    }

    // ════════════════════════════════════════════════════════════
    // 2. 拖拽中 — C 阶段（每帧）
    // ════════════════════════════════════════════════════════════
    public static void OnDrag(UIResponder _this, PointerEventData eventData)
    {
        if (source == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _this.canvas.transform as RectTransform, eventData.position, _this.uiCamera, out Vector2 now);
        Vector2 totalDelta = now - beginPosition;
        (source.transform as RectTransform).anchoredPosition = dragStartPos + totalDelta;
    }

    // ════════════════════════════════════════════════════════════
    // 3. 结算 — D 阶段（手指抬起）
    // ════════════════════════════════════════════════════════════
    public static void EndDrag(UIResponder _this)
    {
        source = null;
    }
}
}
