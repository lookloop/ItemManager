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
        bool isLongPress;
        Coroutine longPressCoroutine;
        // Grid 滑动用
        Vector2 gridPos;
        Vector2 originPos;
        // 拖拽目标
        CellTouch targetCell;
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;


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
                CancelLongPress();
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

        void CancelLongPress()
        {
            if (longPressCoroutine != null)
            {
                StopCoroutine(longPressCoroutine);
                longPressCoroutine = null;
            }
        }

        void Exchange()
        {
            if (targetCell == null) return;

            int srcKey = container.cells.Length * (container.currentPage - 1) + cellKey;
            int tgtKey = targetCell.container.cells.Length * (targetCell.container.currentPage - 1) + targetCell.cellKey;

            core.Exchange(container, srcKey, targetCell.container, tgtKey);
        }

        void Reset()
        {
            if (container != null && container.cells != null &&
                core.sourceContainer == container)
            {
                int start = container.cells.Length * (container.currentPage - 1);
                int end = UnityEngine.Mathf.Min(
                    container.cells.Length * container.currentPage - 1,
                    container.items.Length - 1);
                if (core.sourceItemKey >= start && core.sourceItemKey <= end)
                    core.Launch(
                        core.View(container, core.sourceItemKey));
            }

            targetCell = null;
            lastTurnTime = 0f;
            isDrag = false;
            isLongPress = false;

            core.sourceContainer = null;
            core.sourceItemKey = 0;

            core.dragParent.gameObject.SetActive(false);
            core.Shadow.gameObject.SetActive(false);

            CancelLongPress();
        }
    }
}
