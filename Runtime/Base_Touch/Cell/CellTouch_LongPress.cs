using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    public partial class CellTouch
    {
        float lastTurnTime;

        // ── Long-press coroutine: after pressTime expires, extract the item
        //     then enter a per-frame loop for edge-scroll and edge-turn-page. ──
        IEnumerator LongPressTimer(PointerEventData eventData)
        {
            yield return new WaitForSeconds(core.pressTime);
            isLongPress = true;
            core.Launch(ExtractItem(eventData));
            lastTurnTime = Time.time;

            while (true)
            {
                var targetContainer = targetCell?.container ?? container;
                var maskRect = targetContainer?.maskRect;
                if (maskRect != null)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        maskRect, eventData.position, core.canvas.worldCamera, out Vector2 localPos);
                    ScrollPage(localPos, maskRect, targetContainer);
                    TurnPage(localPos, maskRect, targetContainer);
                }

                yield return null;
            }
        }

        // ── Pull the item out of its cell and show the drag‑ghost icon ──
        async Task ExtractItem(PointerEventData eventData)
        {
            if (container == null || container.items == null) return;
            int globalKey = container.cells.Length * (container.currentPage - 1) + cellKey;
            var item = container.items[globalKey];
            if (item.Id == 0) return;

            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);

            core.dragParent.anchoredPosition = localPos;

            var table = await core.GetItemTable(item.Id.ToString());
            if (table == null) return;

            core.dragItem.sprite = table.ItemSprite;
            core.dragEdge.sprite = table.edgeSprite;
            core.dragCount.text = item.Count.ToString();
            core.dragParent.gameObject.SetActive(true);

            core.NoView(container, globalKey);
        }

        // ── Move the drag‑ghost to follow the finger and raycast for a target cell ──
        void DragItem(PointerEventData eventData)
        {
            if (!core.dragParent.gameObject.activeSelf) return;

            // ghost follows the finger
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);
            core.dragParent.anchoredPosition = localPos;

            // raycast to find the cell under the finger
            var raycast = eventData.pointerCurrentRaycast;
            if (raycast.gameObject != null)
            {
                var other = raycast.gameObject.GetComponent<CellTouch>();
                if (other != null)
                {
                    targetCell = other;

                    if (targetCell.container != null && targetCell.container.containerRect != null)
                        targetCell.container.containerRect.SetAsLastSibling();

                    var cellRect = targetCell.container.cells[targetCell.cellKey].cell;
                    core.Shadow.SetParent(cellRect, false);
                    core.Shadow.gameObject.SetActive(true);
                }
                else
                {
                    ClearTarget();
                }
            }
            else
            {
                ClearTarget();
            }
        }

        void ClearTarget()
        {
            targetCell = null;
            core.Shadow.SetParent(core.canvas.transform, false);
            core.Shadow.gameObject.SetActive(false);
        }

        // ── Edge‑scroll: auto-scroll the grid when the finger is near the
        //     top or bottom edge of the mask (checked every frame). ──
        void ScrollPage(Vector2 localPos, RectTransform maskRect, Container c)
        {
            float maskH = maskRect.rect.height;
            float distTop = Mathf.Abs(localPos.y);
            float distBottom = Mathf.Abs(localPos.y + maskH);
            bool inTop = distTop <= core.flipDistance && distTop < distBottom;
            bool inBottom = distBottom <= core.flipDistance && distBottom < distTop;
            if (!inTop && !inBottom) return;

            var gridRect = c.gridRect;
            float gridH = gridRect.sizeDelta.y;
            float maxY = Mathf.Max(0f, gridH - maskH);
            if (maxY == 0f) return;

            float dir = inTop ? -1f : 1f;
            float targetY = gridRect.anchoredPosition.y + dir * core.scrollSpeed * Time.deltaTime;
            targetY = Mathf.Clamp(targetY, 0f, maxY);
            gridRect.anchoredPosition = new Vector2(gridRect.anchoredPosition.x, targetY);
        }

        // ── Edge‑turn‑page: auto-flip pages when the finger is near the
        //     left or right edge of the mask (checked every frame, with a cooldown). ──
        void TurnPage(Vector2 localPos, RectTransform maskRect, Container c)
        {
            float maskW = maskRect.rect.width;
            float halfW = maskW * 0.5f;
            float distLeft = Mathf.Abs(localPos.x + halfW);
            float distRight = Mathf.Abs(localPos.x - halfW);
            bool inLeft = distLeft <= core.flipDistance && distLeft < distRight;
            bool inRight = distRight <= core.flipDistance && distRight < distLeft;

            if (inLeft || inRight)
            {
                float now = Time.time;
                if (now - lastTurnTime >= core.flipCool)
                {
                    int page = c.currentPage;
                    if (inLeft) page--;
                    if (inRight) page++;
                    core.SetPage(c, page);
                    lastTurnTime = now;

                    // If we flipped back to the same container and landed on
                    // the original page, hide the source cell.
                    if (c == container && container.currentPage == originPage)
                    {
                        int globalKey = container.cells.Length * (originPage - 1) + cellKey;
                        if (globalKey >= 0 && globalKey < container.items.Length)
                            core.NoView(container, globalKey);
                    }
                }
            }
            else
            {
                lastTurnTime = Time.time;
            }
        }
    }
}
