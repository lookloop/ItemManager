namespace Lookloop.ItemManager
{
    public partial class CellTouch
    {
        // ═══════════════════════════════════════════════
        //  交换（长按拖拽松手）
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
            if (srcItem.Id == 0 && tgtItem.Id == 0) return;

            // 双向准入检查
            bool srcOk = srcC.itemFilter == null
                || srcC.itemFilter.CanExchange(tgtItem, srcItem);
            bool tgtOk = tgtC.itemFilter == null
                || tgtC.itemFilter.CanExchange(srcItem, tgtItem);
            if (!srcOk || !tgtOk) return;

            core.SetItem(srcC, srcKey, tgtItem);
            core.SetItem(tgtC, tgtKey, srcItem);
        }

        // ═══════════════════════════════════════════════
        //  重置（每次 OnPointerUp 调用）
        // ═══════════════════════════════════════════════
        void Reset()
        {
            // 恢复 source Cell 显示
            if (container != null && container.cells != null &&
                core.dragSourceContainer == container)
            {
                int start = container.cells.Length * (container.currentPage - 1);
                int end = UnityEngine.Mathf.Min(
                    container.cells.Length * container.currentPage - 1,
                    container.items.Length - 1);
                if (core.dragSourceItemKey >= start && core.dragSourceItemKey <= end)
                    core.FireAndForget(
                        core.View(container, core.dragSourceItemKey));
            }

            targetCell = null;
            lastTurnTime = 0f;
            isDrag = false;
            isLongPress = false;

            core.dragSourceContainer = null;
            core.dragSourceItemKey = 0;

            core.dragRect.gameObject.SetActive(false);
            core.Shadow.gameObject.SetActive(false);

            CancelLongPress();
        }
    }
}
