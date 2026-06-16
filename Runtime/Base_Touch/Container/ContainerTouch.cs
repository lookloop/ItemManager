using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Container 交互 — 挂载在 Container RectTransform 上，处理整体拖拽。
    /// </summary>
    public class ContainerTouch : TouchBase
    {
        Vector2 containerPos;
        Vector2 originPos;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            containerPos = ((RectTransform)transform).anchoredPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform.parent,
                eventData.position,
                core.canvas.worldCamera,
                out originPos);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform.parent,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 offPos);

            ((RectTransform)transform).anchoredPosition =
                containerPos + (offPos - originPos);
        }
    }
}
