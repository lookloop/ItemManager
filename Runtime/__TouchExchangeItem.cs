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

            // 两个都空，无事可做
            if (srcItem == null && tgtItem == null) return;

            // 取出双方数据
            int srcId = srcItem?.Id ?? 0;
            int srcType = srcItem?.Type ?? 0;
            int srcTier = srcItem?.Tier ?? 0;
            int srcCount = srcItem?.Count ?? 0;
            int[] srcData = srcItem?.Data;

            int tgtId = tgtItem?.Id ?? 0;
            int tgtType = tgtItem?.Type ?? 0;
            int tgtTier = tgtItem?.Tier ?? 0;
            int tgtCount = tgtItem?.Count ?? 0;
            int[] tgtData = tgtItem?.Data;

            // 互换写入
            if (tgtItem != null)
                SetItem.Set(core, srcC, srcKey, tgtId, tgtType, tgtTier, tgtCount, tgtData);
            else
                srcC.items[srcKey] = null;

            if (srcItem != null)
                SetItem.Set(core, tgtC, tgtKey, srcId, srcType, srcTier, srcCount, srcData);
            else
                tgtC.items[tgtKey] = null;
        }
    }
}
