using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    public partial class CellTouch
    {
        // ── Short drag (no long-press) → scroll the grid ──
        void ScrollGrid(PointerEventData eventData)
        {
            var gridRect = container.gridRect;
            if (gridRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect.parent as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 offPos);

            Vector2 diff = offPos - originPos;
            float targetY = gridPos.y + diff.y;

            float gridHeight = gridRect.sizeDelta.y;
            float maskHeight = container.maskRect.sizeDelta.y;
            float maxY = Mathf.Max(0f, gridHeight - maskHeight);
            targetY = Mathf.Clamp(targetY, 0f, maxY);

            gridRect.anchoredPosition = new Vector2(gridPos.x, targetY);
        }
    }
}
