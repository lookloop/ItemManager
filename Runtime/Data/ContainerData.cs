using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Runtime container — holds the RectTransform hierarchy, item data, and
    /// a reference back to the spec that built it.
    /// </summary>
    [System.Serializable]
    public class Container
    {
        public int containerIndex;
        public RectTransform containerRect;
        public RectTransform detailRect;
        public RectTransform maskRect;
        public RectTransform gridRect;
        public Cell[] cells;
        public Item[] items;
        public int currentPage = 1;
        public int row;
        public float cellWidth;
        [System.NonSerialized] public DetailBase detailFiller;
        [System.NonSerialized] public TMPro.TMP_InputField pageInput;
        [System.NonSerialized] public SetItemBase itemFilter;
    }
}
