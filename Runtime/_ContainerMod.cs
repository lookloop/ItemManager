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
        public RectTransform[] cells;
        public ItemUIMod[] itemUIs;
        public Item[] items;
        public int currentPage = 1;
    }
}
