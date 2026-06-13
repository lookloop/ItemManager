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
        Vector2 gridStartPos;
        Vector2 fingerStartLocal;
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
                gridStartPos = gridRect.anchoredPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gridRect.parent as RectTransform,
                    eventData.position,
                    core.canvas.worldCamera,
                    out fingerStartLocal);
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
    }
}
