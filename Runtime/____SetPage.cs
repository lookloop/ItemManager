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

            // 检测最后一页
            int totalPages = Mathf.CeilToInt((float)container.items.Length / container.cells.Length);
            if (totalPages > 1 && page == totalPages)
            {
                //执行隐藏cell
                int lastIndex = container.items.Length - 1;
                if (end > lastIndex) end = lastIndex;
                LastPage(container);
            }
            else
            {
                // 恢复满页 grid 高度
                int fullRows = Mathf.CeilToInt((float)container.cells.Length / container.row);
                container.gridRect.sizeDelta = new Vector2(
                    container.gridRect.sizeDelta.x,
                    fullRows * container.cellWidth);

                // 执行显示cell
                for (int i = 0; i < container.cells.Length; i++)
                    container.cells[i].cell.gameObject.SetActive(true);
            }
            //最后不管cell显示还是隐藏，对三个子物体刷新其内容
            for (int i = start; i <= end; i++)
                _ = SetItem.View(core, container, i);
        }

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
