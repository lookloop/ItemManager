using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 容器封装 — Transform + 物品数据
    /// </summary>
    [System.Serializable]
    public class ContainerData
    {
        public RectTransform container;
        public Item[] items;
        /// <summary>当前可见页码（每个容器独立翻页）</summary>
        public int currentPage;
    }
}
