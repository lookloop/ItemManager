using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Container 交互 — 挂载在 Container RectTransform 上，处理整体拖拽。
    /// </summary>
    public class ContainerHandler : TouchBase
    {
        Vector2 originalPos;
        Vector2 pointerOffset;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            originalPos = ((RectTransform)transform).anchoredPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform.parent,
                eventData.position,
                core.canvas.worldCamera,
                out pointerOffset);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform.parent,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 current);

            ((RectTransform)transform).anchoredPosition =
                originalPos + (current - pointerOffset);
        }
    }
}
