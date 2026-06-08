using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// TouchMask — 处理长按拖拽时 Mask 边缘的滚动与翻页行为。
    /// 在 Core.LongPressTimer 的 while(true) 中每帧调用。
    /// </summary>
    public static class TouchMask
    {
        // —— 可调参数 ——
        const float scrollSpeed = 60f;      // 每秒滚轮速度
        const float turnThreshold = 0.3f;   // 翻页积累阈值（秒）
        const float edgeRatio = 0.15f;      // 边缘区域占对应维度的比例

        // —— 翻页积累状态 ——
        static float turnAccum;
        static Container lastContainer;     // 用来检测是否是同一个 container 的连续积累

        public static void EdgeBehavior(Core core)
        {
            // ① 守卫检查
            var container = core.targetContainer;
            if (container == null) return;
            var maskRect = container.maskRect;
            if (maskRect == null) return;

            // ② 手指在 Mask 本地坐标中的位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                maskRect,
                core.eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);

            float maskW = maskRect.rect.width;
            float maskH = maskRect.rect.height;
            float edgeW = maskW * edgeRatio;
            float edgeH = maskH * edgeRatio;

            // ③ 判断手指在哪个边缘区域
            bool inTop    = localPos.y >  maskH - edgeH;
            bool inBottom = localPos.y <  edgeH;
            bool inLeft   = localPos.x <  edgeW;
            bool inRight  = localPos.x >  maskW - edgeW;

            // ④ 上下边缘 → 滚轮
            if (inTop || inBottom)
            {
                // 滚轮时清翻页积累
                ResetTurnAccum();

                var gridRect = container.gridRect;
                if (gridRect == null) return;

                float gridH = gridRect.sizeDelta.y;
                float maxY = Mathf.Max(0f, gridH - maskH);
                if (maxY <= 0f) return; // 无需滚动

                float dir = inTop ? -1f : 1f; // 顶部 → Grid 向下 (Y↓)；底部 → Grid 向上 (Y↑)
                float targetY = gridRect.anchoredPosition.y + dir * scrollSpeed * Time.deltaTime;
                targetY = Mathf.Clamp(targetY, 0f, maxY);

                gridRect.anchoredPosition = new Vector2(gridRect.anchoredPosition.x, targetY);
                return;
            }

            // ⑤ 左右边缘 → 翻页（积累时间）
            if (inLeft || inRight)
            {
                // 同一个 container 才积累，换 container 重置
                if (container != lastContainer)
                {
                    ResetTurnAccum();
                    lastContainer = container;
                }

                turnAccum += Time.deltaTime;
                if (turnAccum >= turnThreshold)
                {
                    turnAccum -= turnThreshold; // 保留溢出，连续触发

                    int totalPages = Mathf.CeilToInt((float)container.items.Length / container.cells.Length);
                    int page = container.currentPage;

                    if (inLeft)  page--;
                    if (inRight) page++;

                    page = Mathf.Clamp(page, 1, totalPages);
                    if (page != container.currentPage)
                        SetPage.Set(core, container, page);
                }
                return;
            }

            // ⑥ 不在任何边缘 → 清积累
            ResetTurnAccum();
        }

        static void ResetTurnAccum()
        {
            turnAccum = 0f;
            lastContainer = null;
        }
    }
}
