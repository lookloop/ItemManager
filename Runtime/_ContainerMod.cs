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
        public Item[] items;
        public RectTransform[] cells;
        public int currentPage;
        public ContainerSpec blueprint;
        public GameObject activeDetailPanel;
    }
}
