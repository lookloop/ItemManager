using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Cell 交互 — 路由 PointerDown/Drag/Up 到各 partial 实现。
    /// 构建时由 Core 注入 core、container、cellKey。
    /// </summary>
    public partial class CellTouch : TouchBase
    {
        [HideInInspector] public int cellKey;

        // ── 会话状态 ──
        bool isDrag;
        internal bool isLongPress;
        Coroutine longPressCoroutine;
        int originPage;
        // Grid 滑动用
        Vector2 gridPos;
        Vector2 originPos;
        CellTouch targetCell;
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            originPage = container.currentPage;

            longPressCoroutine = StartCoroutine(LongPressTimer(eventData));

            // 记录 Grid 起始位置
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
