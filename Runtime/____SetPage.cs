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
            int totalPages = Mathf.CeilToInt((float)container.items.Length / container.cells.Length);
            page = Mathf.Clamp(page, 1, totalPages);
            container.currentPage = page;

            int start = container.cells.Length * (page - 1);
            int end = container.cells.Length * page - 1;

            // 检测最后一页
            if (totalPages > 1 && page == totalPages)
            {
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

                for (int i = 0; i < container.cells.Length; i++)
                    container.cells[i].cell.gameObject.SetActive(true);
            }

            for (int i = start; i <= end; i++)
                _ = SetItem.View(core, container, i);

            // 同步 TMP 翻页输入框显示
            if (container.pageInput != null)
                container.pageInput.text = page + "/" + Mathf.CeilToInt((float)container.items.Length / container.cells.Length);
        }

        static void LastPage(Container container)
        {
            int lastItemCount = container.items.Length % container.cells.Length;
            if (lastItemCount == 0) lastItemCount = container.cells.Length;

            int rows = Mathf.CeilToInt((float)lastItemCount / container.row);
            container.gridRect.sizeDelta = new Vector2(
                container.gridRect.sizeDelta.x,
                rows * container.cellWidth);

            // 高度缩小后，钳制 y 到合法范围
            float gridH = container.gridRect.sizeDelta.y;
            float maskH = container.maskRect.sizeDelta.y;
            float maxY = Mathf.Max(0f, gridH - maskH);
            float y = Mathf.Clamp(container.gridRect.anchoredPosition.y, 0f, maxY);
            container.gridRect.anchoredPosition = new Vector2(container.gridRect.anchoredPosition.x, y);

            for (int i = lastItemCount; i < container.cells.Length; i++)
                container.cells[i].cell.gameObject.SetActive(false);
        }
    }
}
