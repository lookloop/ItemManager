using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Cell = 格子 RectTransform + 物品显示 ItemUI。
    /// 每个 Cell 是一个独立的格子单元，cell 是它的 RectTransform，itemUI 是它的三个显示子对象。
    /// </summary>
    [System.Serializable]
    public class Cell
    {
        public RectTransform cell;
        public Image item;          // ItemUI 自身的 Image（物品图标）
        public Image edge;               // 子级：物品边框/光效
        public TextMeshProUGUI count;
    }
}
