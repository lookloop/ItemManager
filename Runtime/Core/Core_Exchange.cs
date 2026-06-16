namespace Lookloop.ItemManager
{
public partial class Core
{
    /// <summary>
    /// 交换两个容器中的物品（双向准入检查）。
    /// </summary>
    public void Exchange(Container srcC, int srcKey, Container tgtC, int tgtKey)
    {
        if (srcC == null || tgtC == null) return;
        if (srcC.items == null || tgtC.items == null) return;

        var srcItem = srcC.items[srcKey];
        var tgtItem = tgtC.items[tgtKey];
        if (srcItem.Id == 0 && tgtItem.Id == 0) return;

        // 双向准入检查
        bool srcOk = srcC.itemFilter == null
            || srcC.itemFilter.CanExchange(tgtItem, srcItem);
        bool tgtOk = tgtC.itemFilter == null
            || tgtC.itemFilter.CanExchange(srcItem, tgtItem);
        if (!srcOk || !tgtOk) return;

        SetItem(srcC, srcKey, tgtItem);
        SetItem(tgtC, tgtKey, srcItem);
    }
}
}
