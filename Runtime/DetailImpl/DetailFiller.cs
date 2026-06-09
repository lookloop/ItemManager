using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Detail 面板 — 挂载到 detailRect 预制体上，实现 IDetailFiller 接口。
    /// Core 会在物品被选中时自动调用 Fill(container, itemKey)。
    /// </summary>
    public class DetailFiller : MonoBehaviour, IDetailFiller
    {
        public Image fakeItemImage;
        public Image fakeEdgeImage;
        public TMP_Text itemNameText;
        public TMP_Text itemCountText;
        public TMP_Text tierText;
        public TMP_Text attributesText;
        public TMP_Text introductionText;

        public void Fill(Core core, Container container, int itemKey)
        {
            Debug.Log("[DetailFiller] Fill called for key: " + itemKey);
        }
    }
}
