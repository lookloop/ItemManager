using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// A demo filter that only admits items with an odd Id.
    /// Used to verify the [SerializeReference] type drop-down and the
    /// <c>CanExchange</c> / <c>OnItemSet</c> callbacks work end-to-end.
    /// </summary>
    [Serializable]
    public class OddIdOnlyFilter : SetItemBase
    {
        public bool verbose;

        public override bool CanExchange(Item incoming, Item outgoing)
        {
            bool ok = incoming.Id == 0 || incoming.Id % 2 == 1;
            if (verbose && !ok)
                UnityEngine.Debug.Log($"[OddIdOnlyFilter] 拒绝 Id={incoming.Id}");
            return ok;
        }

        public override void OnItemSet(Container container, int itemKey)
        {
            if (verbose)
            {
                var item = container.items[itemKey];
                UnityEngine.Debug.Log(
                    $"[OddIdOnlyFilter] container[{itemKey}] ← Id={item.Id}");
            }
        }
    }
}
