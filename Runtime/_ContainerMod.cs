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
        /// <summary>当前可见页码（每个容器独立翻页）</summary>
        public int currentPage;
        /// <summary>关联的容器蓝图</summary>
        public ContainerSpec blueprint;
        /// <summary>当前激活的详情面板实例（Prefab 模式下）</summary>
        public GameObject activeDetailPanel;
    }
}
