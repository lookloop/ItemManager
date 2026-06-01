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
    }
}
