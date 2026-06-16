using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Runtime item data — a readonly struct that avoids per-element GC allocations
    /// when stored in arrays. An Id of 0 represents an empty slot.
    /// </summary>
    [Serializable]
    public readonly struct Item
    {
        public readonly int Id;
        public readonly int Type;
        public readonly int Tier;
        public readonly int Count;
        public readonly int[] Data;

        public Item(int id, int type, int tier, int count, int[] data)
        {
            Id = id;
            Type = type;
            Tier = tier;
            Count = count;
            Data = data;
        }
    }
}
