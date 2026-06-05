using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// ItemUI 组件引用 — 每个 Cell 有一个 ItemUI，翻页时换数据不换对象。
    /// 只存组件不存 Rect，布局在创建时一次完成。
    /// </summary>
    [System.Serializable]
    public class ItemUIMod
    {
        public Image itemImage;          // ItemUI 自身的 Image（父级图标）
        public Image edge;               // 子级：物品边框/图标
        public TextMeshProUGUI count;    // 子级：数量文字
    }
}
