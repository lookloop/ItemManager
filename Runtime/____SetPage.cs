using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 翻页设置 — 容器分页切换与当前页 Cell 刷新。
    /// </summary>
    public static class SetPage
    {
        public static void Set(Core core, Container container, int page)
        {
            container.currentPage = page;

            int start = container.cells.Length * (page - 1);
            int end = start + container.cells.Length;
            if (end > container.items.Length) end = container.items.Length;

            // 检测最后一页，调整 grid 高度 + 隐藏多余 Cell
            int totalPages = (container.items.Length + container.cells.Length - 1) / container.cells.Length;
            if (page == totalPages)
            {
                int lastItemCount = container.items.Length % container.cells.Length;
                if (lastItemCount == 0) lastItemCount = container.cells.Length;

                int rows = (lastItemCount + container.row - 1) / container.row;
                container.gridRect.sizeDelta = new Vector2(
                    container.gridRect.sizeDelta.x,
                    rows * container.cellWidth);

                // 隐藏超出最后一页物品数量的 Cell
                for (int i = lastItemCount; i < container.cells.Length; i++)
                    container.cells[i].cell.gameObject.SetActive(false);
            }
            else
            {
                // 非最后一页，确保所有 Cell 可见
                for (int i = 0; i < container.cells.Length; i++)
                    container.cells[i].cell.gameObject.SetActive(true);
            }

            for (int i = start; i < end; i++)
                _ = SetItem.View(core, container, i);
        }
    }
}
