using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// TouchCell 的 Grid 辅助 — 处理 Cell 拖拽时与 Grid 相关的坐标换算、位置操作等。
    /// </summary>
    public static class TouchCell_Grid
    {
        public static void On(Core core)
        {
            var gridRect = core.sourceContainer.gridRect;
            if (gridRect == null) return;
            //自己的Pos
            core.sourcePos = gridRect.anchoredPosition;
            //手指的Pos
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect.parent as RectTransform,
                core.eventData.position,
                core.canvas.worldCamera,
                out core.onPos);
        }

        public static void OnDrag(Core core)
        {
            var gridRect = core.sourceContainer.gridRect;
            if (gridRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect.parent as RectTransform,
                core.eventData.position,
                core.canvas.worldCamera,
                out Vector2 newLocal);

            Vector2 diff = newLocal - core.onPos;

            // 最终位置
            float targetY = core.sourcePos.y + diff.y;

            // 钳制：不能低于 0（Grid 顶部不能低于 Mask 顶部）
            //        不能高于 gridHeight - maskHeight（Grid 底部不能超过 Mask 底部）
            float gridHeight = gridRect.sizeDelta.y;
            float maskHeight = core.sourceContainer.maskRect.sizeDelta.y;
            float maxY = Mathf.Max(0f, gridHeight - maskHeight);
            targetY = Mathf.Clamp(targetY, 0f, maxY);

            gridRect.anchoredPosition = new Vector2(core.sourcePos.x, targetY);
        }
    }
}
