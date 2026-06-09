namespace Lookloop.ItemManager
{
    /// <summary>
    /// 物品交换 — 长按拖拽释放时，source 和 target 两个物品互换位置。
    /// 支持跨容器交换。
    /// </summary>
    public static class TouchExchangeItem
    {
        public static void Exchange(Core core)
        {
            if (core.targetItemKey == null) return;

            var srcC = core.sourceContainer;
            var tgtC = core.targetContainer;
            int srcKey = core.sourceItemKey;
            int tgtKey = core.targetItemKey.Value;

            if (srcC == null || tgtC == null) return;
            if (srcC.items == null || tgtC.items == null) return;

            var srcItem = srcC.items[srcKey];
            var tgtItem = tgtC.items[tgtKey];

            if (srcItem == null && tgtItem == null) return;

            SetItem.Set(core, srcC, srcKey, tgtItem);
            SetItem.Set(core, tgtC, tgtKey, srcItem);
        }
    }
}
