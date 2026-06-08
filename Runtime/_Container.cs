using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 容器 — Transform + 物品数据 + 蓝图引用
    /// </summary>
    [System.Serializable]
    public class Container
    {
        public RectTransform rect;
        public RectTransform detail;
        public RectTransform mask;
        public RectTransform grid;
        public Cell[] cells;
        public Item[] items;
        public int currentPage = 1;
        [System.NonSerialized] public IDetailFiller detailFiller;
    }
}
