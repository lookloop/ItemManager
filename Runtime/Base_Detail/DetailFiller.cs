using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Default detail panel implementation. Shown when the player taps an item;
    /// hidden automatically by <c>Core.OnPointerDown</c> when the player taps
    /// empty space.
    /// </summary>
    public class DetailFiller : DetailBase
    {
        public Image fakeItemImage;
        public Image fakeEdgeImage;
        public TMP_Text itemNameText;
        public TMP_Text itemCountText;
        public TMP_Text tierText;
        public TMP_Text attributesText;
        public TMP_Text introductionText;

        public override async Task Fill(Core core, Container container, int itemKey)
        {
            try
            {
                var item = container.items[itemKey];
                if (item.Id == 0) return;

                var table = await core.GetItemTable(item.Id.ToString());
                if (table == null) return;

                fakeItemImage.sprite = table.ItemSprite;
                fakeEdgeImage.sprite = table.edgeSprite;
                itemNameText.text = table.ItemName;
                itemCountText.text = item.Count.ToString();
                tierText.text = item.Tier.ToString();
                introductionText.text = table.ItemDescription;

                PositionAtCell(core, container, itemKey);
                gameObject.SetActive(true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DetailFiller] Fill 异常: {ex}");
            }
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
