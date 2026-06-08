namespace Lookloop.ItemManager
{
    /// <summary>
    /// 物品设置 — 创建 Item 并写入容器的 items 数组指定位置。
    /// </summary>
    public static class SetItem
    {
        public static void Set(Container container, int itemKey,
            int id, int type, int tier, int count, int[] data)
        {
            container.items[itemKey] = new Item(id, type, tier, count, data);
        }
    }
}
