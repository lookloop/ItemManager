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
        public Item[] items;
        //翻译:当前页面
        public int currentPage;
        
    }
}
