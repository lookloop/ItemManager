using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Per-cell touch handler. Routes <c>PointerDown</c> / <c>Drag</c> / <c>Up</c>
    /// events to the corresponding partial methods.
    ///
    /// References to <c>core</c>, <c>container</c>, and <c>cellKey</c> are injected
    /// by the container builder at construction time.
    /// </summary>
    public partial class CellTouch : TouchBase
    {
        [HideInInspector] public int cellKey;

        // ── Per‑drag session state ──
        bool isDrag;
        internal bool isLongPress;
        Coroutine longPressCoroutine;
        int originPage;
        // Grid scroll tracking
        Vector2 gridPos;
        Vector2 originPos;
        CellTouch targetCell;
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            originPage = container.currentPage;

            longPressCoroutine = StartCoroutine(LongPressTimer(eventData));

            // Capture the grid's initial position for scroll calculations
            var gridRect = container.gridRect;
            if (gridRect != null)
            {
                gridPos = gridRect.anchoredPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gridRect.parent as RectTransform,
                    eventData.position,
                    core.canvas.worldCamera,
                    out originPos);
            }
        }
        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;
            isDrag = true;

            if (isLongPress)
                DragItem(eventData);
            else
            {
                if (longPressCoroutine != null)
                {
                    StopCoroutine(longPressCoroutine);
                    longPressCoroutine = null;
                }
                ScrollGrid(eventData);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            if (isLongPress && isDrag)
                Exchange();
            else if (!isLongPress && !isDrag)
                ShowDetail();

            Reset();
        }

        void Exchange()
        {
            if (targetCell == null) return;

            int srcKey = container.cells.Length * (originPage - 1) + cellKey;
            int tgtKey = targetCell.container.cells.Length * (targetCell.container.currentPage - 1) + targetCell.cellKey;

            core.Exchange(container, srcKey, targetCell.container, tgtKey);
        }

        void Reset()
        {
            if (isLongPress && container.currentPage == originPage)
            {
                int globalKey = container.cells.Length * (originPage - 1) + cellKey;
                core.Launch(core.View(container, globalKey));
            }

            targetCell = null;
            lastTurnTime = 0f;
            isDrag = false;
            isLongPress = false;

            core.dragParent.gameObject.SetActive(false);
            core.Shadow.gameObject.SetActive(false);

            if (longPressCoroutine != null)
            {
                StopCoroutine(longPressCoroutine);
                longPressCoroutine = null;
            }
        }
    }
}
