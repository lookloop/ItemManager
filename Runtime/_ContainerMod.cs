using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 容器模块 — Transform + 物品数据 + 蓝图引用
    /// </summary>
    [System.Serializable]
    public class ContainerMod
    {
        public RectTransform container;
        public RectTransform detail;
        public RectTransform mask;
        public RectTransform grid;
        public Cell[] cells;
        public Item[] items;
        public int currentPage = 1;
        /// <summary>detail 面板的填充接口引用，ContainerBuilder 构建时缓存</summary>
        [System.NonSerialized] public IDetailFiller detailFiller;
    }
}
