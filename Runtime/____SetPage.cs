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

            for (int i = start; i < end; i++)
                _ = SetItem.View(core, container, i);
        }
    }
}
