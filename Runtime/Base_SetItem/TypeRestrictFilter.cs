using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Restricts a container so it only accepts items whose <c>Type</c> is in the
    /// whitelist. Useful for equipment slots, consumable slots, etc.
    ///
    /// <example>
    /// Inspector setup:
    ///   Allowed Types  [1] [2]        ← only Type == 1 or 2 may enter
    /// </example>
    /// </summary>
    [Serializable]
    public class TypeRestrictFilter : SetItemBase
    {
        public int[] allowedTypes;

        public override bool CanExchange(Item incoming, Item outgoing)
        {
            // Clearing a slot or incoming empty hand — always allowed
            if (incoming.Id == 0) return true;

            if (allowedTypes == null || allowedTypes.Length == 0)
                return true; // no whitelist configured — allow everything

            return Array.IndexOf(allowedTypes, incoming.Type) >= 0;
        }
    }
}
