using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// TouchCell 的 Grid 辅助 — 处理 Cell 拖拽时与 Grid 相关的坐标换算、位置操作等。
    /// </summary>
    public static class TouchCell_Grid
    {
        static Vector2 gridSourcePos;
        static Vector2 gridOnPos;
        static bool gridInited;

        public static void OnDrag(Core core, PointerEventData eventData)
        {
            var gridRect = core.sourceContainer.gridRect;
            if (gridRect == null) return;

            // 首次拖拽时记录 grid 的起始位置和本地坐标
            if (!gridInited)
            {
                gridSourcePos = gridRect.anchoredPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gridRect.parent as RectTransform,
                    eventData.position,
                    core.canvas.worldCamera,
                    out gridOnPos);
                gridInited = true;
            }

            // 新坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect.parent as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 newLocal);

            // 距离 = 新坐标 - 旧坐标
            Vector2 diff = newLocal - gridOnPos;

            // Grid 位置 = 起始位置 + 垂直方向的位移
            gridRect.anchoredPosition = new Vector2(gridSourcePos.x, gridSourcePos.y + diff.y);
        }

        public static void ResetGrid()
        {
            gridInited = false;
        }
    }
}
