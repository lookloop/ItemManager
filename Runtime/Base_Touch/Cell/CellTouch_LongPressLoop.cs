using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    public partial class CellTouch
    {
        float lastTurnTime;

        // ═══════════════════════════════════════════════
        //  长按协程
        // ═══════════════════════════════════════════════
        IEnumerator LongPressTimer(PointerEventData eventData)
        {
            yield return new WaitForSeconds(core.pressTime);
            isLongPress = true;
            core.FireAndForget(ExtractItem(eventData));
            lastTurnTime = Time.time;

            while (true)
            {
                var edgeC = targetCell?.container ?? container;
                var edgeMask = edgeC?.maskRect;
                if (edgeMask != null)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        edgeMask, eventData.position, core.canvas.worldCamera, out Vector2 lp);
                    ScrollPageByEdge(lp, edgeMask, edgeC);
                    TurnPageByEdge(lp, edgeMask, edgeC);
                }
                yield return null;
            }
        }

        // ═══════════════════════════════════════════════
        //  Mask 边缘滚动（长按时每帧检测）
        // ═══════════════════════════════════════════════
        void ScrollPageByEdge(Vector2 localPos, RectTransform maskRect, Container c)
        {
            float maskH = maskRect.rect.height;
            float distTop = Mathf.Abs(localPos.y);
            float distBottom = Mathf.Abs(localPos.y + maskH);
            bool inTop = distTop <= core.edgeThreshold && distTop < distBottom;
            bool inBottom = distBottom <= core.edgeThreshold && distBottom < distTop;
            if (!inTop && !inBottom) return;

            var gridRect = c.gridRect;
            float gridH = gridRect.sizeDelta.y;
            float maxY = Mathf.Max(0f, gridH - maskH);
            if (maxY <= 0f) return;

            float dir = inTop ? -1f : 1f;
            float targetY = gridRect.anchoredPosition.y + dir * core.scrollSpeed * Time.deltaTime;
            targetY = Mathf.Clamp(targetY, 0f, maxY);
            gridRect.anchoredPosition = new Vector2(gridRect.anchoredPosition.x, targetY);
        }

        // ═══════════════════════════════════════════════
        //  边缘翻页检测（长按时每帧检测）
        // ═══════════════════════════════════════════════
        void TurnPageByEdge(Vector2 localPos, RectTransform maskRect, Container c)
        {
            float maskW = maskRect.rect.width;
            float halfW = maskW * 0.5f;
            float distLeft = Mathf.Abs(localPos.x + halfW);
            float distRight = Mathf.Abs(localPos.x - halfW);
            bool inLeft = distLeft <= core.edgeThreshold && distLeft < distRight;
            bool inRight = distRight <= core.edgeThreshold && distRight < distLeft;

            if (inLeft || inRight)
            {
                float now = Time.time;
                if (now - lastTurnTime >= core.turnThreshold)
                {
                    int page = c.currentPage;
                    if (inLeft) page--;
                    if (inRight) page++;
                    core.SetPage(c, page);
                    lastTurnTime = now;
                }
            }
        }
    }
}
