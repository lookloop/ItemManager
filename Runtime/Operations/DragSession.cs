namespace Lookloop.ItemManager
{
    /// <summary>
    /// 拖拽会话 — 跨组件共享的拖拽状态。
    /// CellHandler 在提取物品时写入，SetPage 翻页时读取以隐藏源格子。
    /// </summary>
    public static class DragSession
    {
        public static Container sourceContainer;
        public static int sourceItemKey;

        public static void Begin(Container container, int itemKey)
        {
            sourceContainer = container;
            sourceItemKey = itemKey;
        }

        public static void End()
        {
            sourceContainer = null;
            sourceItemKey = 0;
        }
    }
}
