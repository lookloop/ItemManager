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

        public static void ScrollPage(Core core)
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

            float maskH = maskRect.rect.height;
            float edgeH = maskH * edgeRatio;

            float distTop    = Mathf.Abs(localPos.y);
            float distBottom = Mathf.Abs(localPos.y + maskH);

            bool inTop    = distTop    <= edgeH && distTop    < distBottom;
            bool inBottom = distBottom <= edgeH && distBottom < distTop;

            if (!inTop && !inBottom) return;

            var gridRect = container.gridRect;
            float gridH = gridRect.sizeDelta.y;
            float maxY = Mathf.Max(0f, gridH - maskH);
            if (maxY <= 0f) return;

            float dir = inTop ? -1f : 1f;
            float targetY = gridRect.anchoredPosition.y + dir * scrollSpeed * Time.deltaTime;
            targetY = Mathf.Clamp(targetY, 0f, maxY);

            gridRect.anchoredPosition = new Vector2(gridRect.anchoredPosition.x, targetY);
        }

        public static void TurnPage(Core core)
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
            float halfW = maskW * 0.5f;

            // Mask pivot (0.5, 1)，x 原点在中心：x=-halfW 左边缘，x=+halfW 右边缘
            float distLeft  = Mathf.Abs(localPos.x + halfW);
            float distRight = Mathf.Abs(localPos.x - halfW);

            bool inLeft  = distLeft  <= edgeW && distLeft  < distRight;
            bool inRight = distRight <= edgeW && distRight < distLeft;

            if (inLeft || inRight)
            {
                float now = Time.time;
                if (now - lastTurnTime >= turnThreshold)
                {
                    int page = container.currentPage;
                    if (inLeft)  page--;
                    if (inRight) page++;
                    SetPage.Set(core, container, page);
                    lastTurnTime = now;
                }
            }
        }
    }
}
