using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 物品运行时数据 — 纯值类型，避免数组存取产生 GC 分配。
    /// Id == 0 视为空槽位。
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
