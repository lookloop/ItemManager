using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Test filter: only allows incoming items with Id 1 or 3.
    /// 2 and 4 are rejected.
    /// </summary>
    [Serializable]
    public class OccupiedRejectFilter : SetItemBase
    {
        public override bool CanExchange(Item incoming, Item outgoing)
        {
            return incoming.Id == 1 || incoming.Id == 3;
        }
    }
}
