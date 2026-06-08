using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 容器 — Transform + 物品数据 + 蓝图引用
    /// </summary>
    [System.Serializable]
    public class Container
    {
        public RectTransform containerRect;
        public RectTransform detailRect;
        public RectTransform maskRect;
        public RectTransform gridRect;
        public Cell[] cells;
        public Item[] items;
        public int currentPage = 1;
        public int row;
        public float cellWidth;
        [System.NonSerialized] public IDetailFiller detailFiller;
    }
}
