namespace Lookloop.ItemManager
{
    /// <summary>
    /// 拖拽会话 — 跨组件共享的拖拽状态。
    /// CellHandler 在提取物品时写入，SetPage 翻页时读取以隐藏源格子。
    /// 每个 Core 持有一个实例，避免多容器场景下的静态状态冲突。
    /// </summary>
    public class DragSession
    {
        public Container sourceContainer;
        public int sourceItemKey;

        public void Begin(Container container, int itemKey)
        {
            sourceContainer = container;
            sourceItemKey = itemKey;
        }

        public void End()
        {
            sourceContainer = null;
            sourceItemKey = 0;
        }
    }
}
