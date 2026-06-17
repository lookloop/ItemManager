namespace Lookloop.ItemManager
{
public partial class Core
{
    /// <summary>
    /// Swap items between two containers after a bidirectional admission check.
    /// Both the source and target filters must approve the exchange.
    /// </summary>
    public void Exchange(Container srcC, int srcKey, Container tgtC, int tgtKey)
    {
        if (srcC == null || tgtC == null) return;
        if (srcC.items == null || tgtC.items == null) return;

        var srcItem = srcC.items[srcKey];
        var tgtItem = tgtC.items[tgtKey];
        if (srcItem.Id == 0 && tgtItem.Id == 0) return;

        // Bidirectional admission check
        bool srcOk = srcC.itemFilter == null
            || srcC.itemFilter.CanExchange(tgtItem, srcItem);
        bool tgtOk = tgtC.itemFilter == null
            || tgtC.itemFilter.CanExchange(srcItem, tgtItem);
        if (!srcOk || !tgtOk)
        {
            ShowTip("交换失败", 0.5f, UnityEngine.Color.red);
            return;
        }

        SetItem(srcC, srcKey, tgtItem);
        SetItem(tgtC, tgtKey, srcItem);
        ShowTip("交换成功", 0.5f, UnityEngine.Color.green);
    }
}
}
