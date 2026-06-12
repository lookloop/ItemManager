using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Cell 交互 — 直接挂载在每个格子 RectTransform 的 GameObject 上。
    /// 吸收原 TouchCell / TouchCell_Grid / TouchItem / TouchMask / TouchExchangeItem 全部逻辑。
    /// 构建时由 ContainerBuilder 注入 core、container、cellKey。
    /// </summary>
    public class CellHandler : TouchBase
    {
        [HideInInspector] public int cellKey;

        // ── 会话状态 ──
        bool isDrag;
        bool isLongPress;
        Coroutine longPressCoroutine;
        float lastTurnTime;

        // Grid 滑动用
        Vector2 gridStartPos;
        Vector2 fingerStartLocal;

        // 拖拽目标（通过射线命中对方的 CellHandler 直接拿到引用）
        CellHandler targetCell;

        // ═══════════════════════════════════════════════
        //  按下
        // ═══════════════════════════════════════════════
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            // 启动长按计时
            longPressCoroutine = StartCoroutine(LongPressTimer(eventData));

            // 记录 Grid 起始位置，供后续滑动使用
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

        // ═══════════════════════════════════════════════
        //  拖拽
        // ═══════════════════════════════════════════════
        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;
            isDrag = true;

            if (isLongPress)
            {
                // 长按拖拽：移动幽灵图标 + 射线找目标
                DragItem(eventData);
            }
            else
            {
                // 短按拖拽：grid 滑动
                CancelLongPress();
                ScrollGrid(eventData);
            }
        }

        // ═══════════════════════════════════════════════
        //  释放
        // ═══════════════════════════════════════════════
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            if (isLongPress && isDrag)
            {
                // 长按拖拽松手 → 交换物品
                Exchange();
            }
            else if (!isLongPress && !isDrag)
            {
                // 纯点击 → 显示详情
                int globalKey = container.cells.Length * (container.currentPage - 1) + cellKey;
                TaskSafeguard.FireAndForget(container.detailFiller?.Fill(core, container, globalKey) ?? Task.CompletedTask);
            }

            Reset();
        }

        // ═══════════════════════════════════════════════
        //  长按协程
        // ═══════════════════════════════════════════════
        IEnumerator LongPressTimer(PointerEventData eventData)
        {
            yield return new WaitForSeconds(core.pressTime);
            isLongPress = true;
            TaskSafeguard.FireAndForget(ExtractItem(eventData));
            lastTurnTime = Time.time;

            while (true)
            {
                ScrollPageByEdge(eventData);
                TurnPageByEdge(eventData);
                yield return null;
            }
        }

        void CancelLongPress()
        {
            if (longPressCoroutine != null)
            {
                StopCoroutine(longPressCoroutine);
                longPressCoroutine = null;
            }
        }

        // ═══════════════════════════════════════════════
        //  提取物品 → 幽灵图标
        // ═══════════════════════════════════════════════
        async Task ExtractItem(PointerEventData eventData)
        {
            if (container == null || container.items == null) return;
            int globalKey = container.cells.Length * (container.currentPage - 1) + cellKey;
            var item = container.items[globalKey];
            if (item == null || item.Id == 0) return;

            core.dragSession.Begin(container, globalKey);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);

            core.dragTool.dragRect.anchoredPosition = localPos;

            var table = await core.GetItemTable(item.Id.ToString());
            if (table == null) return;

            core.dragTool.dragItem.sprite = table.ItemSprite;
            core.dragTool.dragEdge.sprite = table.GlowSprite;
            core.dragTool.dragCount.text = item.Count.ToString();
            core.dragTool.dragRect.gameObject.SetActive(true);

            SetItem.NoView(container, globalKey);
        }

        // ═══════════════════════════════════════════════
        //  拖拽幽灵 + 射线找目标
        // ═══════════════════════════════════════════════
        void DragItem(PointerEventData eventData)
        {
            if (!core.dragTool.dragRect.gameObject.activeSelf) return;

            // 幽灵跟随手指
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 localPos);
            core.dragTool.dragRect.anchoredPosition = localPos;

            // 射线找目标
            var raycast = eventData.pointerCurrentRaycast;
            if (raycast.gameObject != null)
            {
                var otherHandler = raycast.gameObject.GetComponent<CellHandler>();
                if (otherHandler != null && otherHandler != this)
                {
                    targetCell = otherHandler;

                    // 高亮当前层级
                    if (targetCell.container != null && targetCell.container.containerRect != null)
                        targetCell.container.containerRect.SetAsLastSibling();

                    // Shadow 挂到目标格子下
                    var cellRect = targetCell.container.cells[targetCell.cellKey].cell;
                    core.dragTool.Shadow.SetParent(cellRect, false);
                    core.dragTool.Shadow.gameObject.SetActive(true);
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
            core.dragTool.Shadow.SetParent(core.canvas.transform, false);
            core.dragTool.Shadow.gameObject.SetActive(false);
        }

        // ═══════════════════════════════════════════════
        //  Grid 滑动
        // ═══════════════════════════════════════════════
        void ScrollGrid(PointerEventData eventData)
        {
            var gridRect = container.gridRect;
            if (gridRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect.parent as RectTransform,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 currentLocal);

            Vector2 diff = currentLocal - fingerStartLocal;
            float targetY = gridStartPos.y + diff.y;

            float gridHeight = gridRect.sizeDelta.y;
            float maskHeight = container.maskRect.sizeDelta.y;
            float maxY = Mathf.Max(0f, gridHeight - maskHeight);
            targetY = Mathf.Clamp(targetY, 0f, maxY);

            gridRect.anchoredPosition = new Vector2(gridStartPos.x, targetY);
        }

        // ═══════════════════════════════════════════════
        //  长按时 Mask 边缘滚动 + 翻页
        // ═══════════════════════════════════════════════
        void ScrollPageByEdge(PointerEventData eventData)
        {
            var c = targetCell?.container ?? container;
            var maskRect = c?.maskRect;
            if (maskRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                maskRect, eventData.position, core.canvas.worldCamera, out Vector2 localPos);

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

        void TurnPageByEdge(PointerEventData eventData)
        {
            var c = targetCell?.container ?? container;
            var maskRect = c?.maskRect;
            if (maskRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                maskRect, eventData.position, core.canvas.worldCamera, out Vector2 localPos);

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
                    SetPage.Set(core, c, page);
                    lastTurnTime = now;
                }
            }
        }

        // ═══════════════════════════════════════════════
        //  交换
        // ═══════════════════════════════════════════════
        void Exchange()
        {
            if (targetCell == null) return;

            var srcC = container;
            var tgtC = targetCell.container;
            if (srcC == null || tgtC == null) return;
            if (srcC.items == null || tgtC.items == null) return;

            int srcKey = srcC.cells.Length * (srcC.currentPage - 1) + cellKey;
            int tgtKey = tgtC.cells.Length * (tgtC.currentPage - 1) + targetCell.cellKey;

            var srcItem = srcC.items[srcKey];
            var tgtItem = tgtC.items[tgtKey];
            if (srcItem == null && tgtItem == null) return;

            SetItem.Set(core, srcC, srcKey, tgtItem);
            SetItem.Set(core, tgtC, tgtKey, srcItem);
        }

        // ═══════════════════════════════════════════════
        //  重置
        // ═══════════════════════════════════════════════
        void Reset()
        {
            // 恢复 source Cell 显示（仅当在当前页）
            if (container != null && container.cells != null)
            {
                int start = container.cells.Length * (container.currentPage - 1);
                int end = container.cells.Length * container.currentPage - 1;
                int globalKey = start + cellKey;
                if (globalKey >= start && globalKey <= end)
                    TaskSafeguard.FireAndForget(SetItem.View(core, container, globalKey));
            }

            targetCell = null;
            lastTurnTime = 0f;
            isDrag = false;
            isLongPress = false;

            core.dragSession.End();

            core.dragTool.dragRect.gameObject.SetActive(false);
            core.dragTool.Shadow.gameObject.SetActive(false);

            CancelLongPress();
        }
    }
}
