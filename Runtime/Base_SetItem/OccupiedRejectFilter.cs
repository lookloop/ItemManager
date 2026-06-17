using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Rejects any exchange where the slot is already occupied.
    /// Only empty slots (outgoing.Id == 0) accept incoming items.
    /// </summary>
    [Serializable]
    public class OccupiedRejectFilter : SetItemBase
    {
        public override bool CanExchange(Item incoming, Item outgoing)
        {
            return outgoing.Id == 0;
        }
    }
}
