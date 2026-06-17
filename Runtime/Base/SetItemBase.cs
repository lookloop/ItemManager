using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Base class for container-admission filters. Marked with [SerializeReference]
    /// so derived types appear in a drop-down inside the Inspector and their fields
    /// are expanded inline.
    ///
    /// <example>
    /// An equipment slot that only accepts weapons:
    /// <code>
    /// [Serializable]
    /// public class WeaponOnlyFilter : SetItemBase
    /// {
    ///     public int[] allowedTypes;
    ///
    ///     public override bool CanExchange(Item incoming, Item outgoing)
    ///         => incoming.Id == 0
    ///         || Array.IndexOf(allowedTypes, incoming.Type) >= 0;
    /// }
    /// </code>
    ///
    /// Assign at runtime with a single line:
    /// <code>
    /// container.itemFilter = new WeaponOnlyFilter { allowedTypes = new[] { 1, 2 } };
    /// </code>
    /// </example>
    /// </summary>
    [Serializable]
    public class SetItemBase
    {
        [System.NonSerialized] public Core core;
        [System.NonSerialized] public Container container;

        /// <summary>
        /// Pre-exchange admission check. Called bidirectionally during a swap.
        /// </summary>
        /// <param name="incoming">The item being placed into this container
        /// (Id == 0 means the other side is empty-handed / an empty slot).</param>
        /// <param name="outgoing">The item being removed from this container
        /// (Id == 0 means this slot is currently empty).</param>
        /// <returns><c>true</c> to allow the exchange, <c>false</c> to block it.</returns>
        public virtual bool CanExchange(Item incoming, Item outgoing)
        {
            return true;
        }

        /// <summary>
        /// Callback invoked after <c>SetItem</c> has written data into this container.
        /// </summary>
        /// <param name="container">The container that was mutated.</param>
        /// <param name="itemKey">The global item index that was changed.</param>
        public virtual void OnItemSet(Container container, int itemKey) { }
    }
}
