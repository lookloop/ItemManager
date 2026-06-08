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
            int end = container.cells.Length * page - 1;
            int lastIndex = container.items.Length - 1;
            if (end > lastIndex) end = lastIndex;

            // 检测最后一页
            int totalPages = Mathf.CeilToInt((float)container.items.Length / container.cells.Length);
            if (totalPages > 1 && page == totalPages)
                LastPage(container);
            else
                for (int i = 0; i < container.cells.Length; i++)
                    container.cells[i].cell.gameObject.SetActive(true);

            for (int i = start; i <= end; i++)
                _ = SetItem.View(core, container, i);
        }

        /// <summary>
        /// 最后一页：调整 grid 高度 + 隐藏多余 Cell。
        /// 仅在总页数上限 > 1 且当前为最后一页时调用。
        /// </summary>
        static void LastPage(Container container)
        {
            int lastItemCount = container.items.Length % container.cells.Length;
            if (lastItemCount == 0) lastItemCount = container.cells.Length;

            int rows = Mathf.CeilToInt((float)lastItemCount / container.row);
            container.gridRect.sizeDelta = new Vector2(
                container.gridRect.sizeDelta.x,
                rows * container.cellWidth);

            for (int i = lastItemCount; i < container.cells.Length; i++)
                container.cells[i].cell.gameObject.SetActive(false);
        }
    }
}
