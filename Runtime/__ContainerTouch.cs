using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
/// <summary>
/// Container 触控总成 — 面板拖拽。
/// </summary>
public static class ContainerTouch
{
    public static RectTransform source;
    static Vector2 beginPosition;
    static Vector2 dragStartPos;


    public static void BeginDrag(Core core, PointerEventData eventData)
    {
        source = eventData.pointerCurrentRaycast.gameObject.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            core.canvas.transform as RectTransform, eventData.position, core.canvas?.worldCamera, out beginPosition);
        dragStartPos = source.anchoredPosition;
    }


    public static void OnDrag(Core core, PointerEventData eventData)
    {
        if (source == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            core.canvas.transform as RectTransform, eventData.position, core.canvas?.worldCamera, out Vector2 now);
        Vector2 totalDelta = now - beginPosition;
        if (totalDelta.sqrMagnitude > 0.01f)
            source.anchoredPosition = dragStartPos + totalDelta;
    }
    public static void EndDrag(Core core)
    {
        source = null;
    }
}
}
