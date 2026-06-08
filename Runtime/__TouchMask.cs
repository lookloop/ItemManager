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

            // Mask pivot (0.5, 1)，原点在顶部中心：y=0 是顶部，y=-maskH 是底部
            bool inTop    = localPos.y > -edgeH;
            bool inBottom = localPos.y < -(maskH - edgeH);

            if (!inTop && !inBottom) return;

            float gridH = gridRect.sizeDelta.y;
            float maxY = Mathf.Max(0f, gridH - maskH);
            if (maxY <= 0f) return;

            float dir = inTop ? -1f : 1f;
            float targetY = gridRect.anchoredPosition.y + dir * scrollSpeed * Time.deltaTime;
            targetY = Mathf.Clamp(targetY, 0f, maxY);

            gridRect.anchoredPosition = new Vector2(gridRect.anchoredPosition.x, targetY);
        }
    }
}
