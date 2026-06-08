using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// TouchMask — 长按拖拽时 Mask 边缘滚动行为。
    /// 在 Core.LongPressTimer 的 while(true) 中每帧调用。
    /// </summary>
    public static class TouchMask
    {
        const float scrollSpeed = 60f;
        const float edgeRatio = 0.15f;
        const float turnThreshold = 0.3f;

        static float lastTurnTime;

        public static void EdgeBehavior(Core core)
        {
            var container = core.targetContainer;
            if (container == null) return;
            var maskRect = container.maskRect;
            if (maskRect == null) return;
            var gridRect = container.gridRect;
            if (gridRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                maskRect,
                core.eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);

            float maskH = maskRect.rect.height;
            float edgeH = maskH * edgeRatio;

            // 到顶部的距离（y=0）、到底部的距离（y=-maskH），取绝对值判断
            float distTop    = Mathf.Abs(localPos.y);           // |y - 0|
            float distBottom = Mathf.Abs(localPos.y + maskH);   // |y - (-maskH)|

            bool inTop    = distTop    <= edgeH && distTop    < distBottom;
            bool inBottom = distBottom <= edgeH && distBottom < distTop;

            if (!inTop && !inBottom) return;

            float gridH = gridRect.sizeDelta.y;
            float maxY = Mathf.Max(0f, gridH - maskH);
            if (maxY <= 0f) return;

            float dir = inTop ? -1f : 1f;
            float targetY = gridRect.anchoredPosition.y + dir * scrollSpeed * Time.deltaTime;
            targetY = Mathf.Clamp(targetY, 0f, maxY);

            gridRect.anchoredPosition = new Vector2(gridRect.anchoredPosition.x, targetY);
        }

        public static void TurnPageBehavior(Core core)
        {
            var container = core.targetContainer;
            if (container == null) return;
            var maskRect = container.maskRect;
            if (maskRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                maskRect,
                core.eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);

            float maskW = maskRect.rect.width;
            float edgeW = maskW * edgeRatio;

            float distLeft  = Mathf.Abs(localPos.x);
            float distRight = Mathf.Abs(localPos.x - maskW);

            bool inLeft  = distLeft  <= edgeW && distLeft  < distRight;
            bool inRight = distRight <= edgeW && distRight < distLeft;

            float now = Time.time;

            if (inLeft || inRight)
            {
                if (now - lastTurnTime >= turnThreshold)
                {
                    int page = container.currentPage;
                    if (inLeft)  page--;
                    if (inRight) page++;
                    SetPage.Set(core, container, page);
                }
            }

            lastTurnTime = now;
        }
    }
}
