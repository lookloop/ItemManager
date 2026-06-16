using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Enables dragging an entire container around on the canvas.
    /// Attached to the container's root RectTransform at build time.
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
