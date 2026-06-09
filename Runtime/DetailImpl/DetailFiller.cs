using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Detail 面板 — 点击物品时显示详情。
    /// 点击空白隐藏由 Core.OnPointerDown 统一处理。
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

        public async void Fill(Core core, Container container, int itemKey)
        {
            var item = container.items[itemKey];
            if (item == null || item.Id == 0) return;

            var table = await core.GetItemTable(item.Id.ToString());
            if (table == null) return;

            fakeItemImage.sprite = table.ItemSprite;
            fakeEdgeImage.sprite = table.GlowSprite;
            itemNameText.text = table.ItemName;
            itemCountText.text = item.Count.ToString();
            tierText.text = item.Tier.ToString();
            introductionText.text = table.ItemDescription;

            PositionAtCell(core, container, itemKey);
            gameObject.SetActive(true);
        }

        void PositionAtCell(Core core, Container container, int itemKey)
        {
            int cellKey = itemKey - container.cells.Length * (container.currentPage - 1);
            if (cellKey < 0 || cellKey >= container.cells.Length) return;

            RectTransform itemRect = container.cells[cellKey].item.rectTransform;
            RectTransform rt = GetComponent<RectTransform>();
            RectTransform parentRt = rt.parent as RectTransform;
            Camera cam = core.canvas.worldCamera;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, itemRect.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRt, screenPoint, cam, out Vector2 localPos);

            float cellW = itemRect.sizeDelta.x;
            Vector2 pivot = rt.pivot;
            if (localPos.x <= 0f)
            {
                pivot.x = 0f;
                localPos.x += cellW * 0.5f;
            }
            else
            {
                pivot.x = 1f;
                localPos.x -= cellW * 0.5f;
            }
            rt.pivot = pivot;
            rt.anchoredPosition = localPos;
        }
    }
}
