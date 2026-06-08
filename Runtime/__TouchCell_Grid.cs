using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// TouchCell 的 Grid 辅助 — 处理 Cell 拖拽时与 Grid 相关的坐标换算、位置操作等。
    /// </summary>
    public static class TouchCell_Grid
    {
        public static void On(Core core, PointerEventData eventData)
        {
            var gridRect = core.sourceContainer.gridRect;
            if (gridRect == null) return;
            //自己的Pos
            core.sourcePos = gridRect.anchoredPosition;
            //手指的Pos
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect.parent as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out core.onPos);
        }

        public static void OnDrag(Core core, PointerEventData eventData)
        {
            var gridRect = core.sourceContainer.gridRect;
            if (gridRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect.parent as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 newLocal);

            Vector2 diff = newLocal - core.onPos;

            gridRect.anchoredPosition = new Vector2(core.sourcePos.x, core.sourcePos.y + diff.y);
        }
    }
}
