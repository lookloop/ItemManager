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

            int srcKey = container.cells.Length * (container.currentPage - 1) + cellKey;
            int tgtKey = targetCell.container.cells.Length * (targetCell.container.currentPage - 1) + targetCell.cellKey;

            core.Exchange(container, srcKey, targetCell.container, tgtKey);
        }

        // ═══════════════════════════════════════════════
        //  重置（每次 OnPointerUp 调用）
        // ═══════════════════════════════════════════════
        void Reset()
        {
            // 恢复 source Cell 显示
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
