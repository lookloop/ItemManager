using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// A single inventory cell — its RectTransform plus the three child views
    /// that render an item: icon, border, and count label.
    /// </summary>
    [System.Serializable]
    public class Cell
    {
        public RectTransform cell;
        public Image item;          // item icon
        public Image edge;          // border / glow frame
        public TextMeshProUGUI count;
    }
}
