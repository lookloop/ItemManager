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
            PositionAtCell(core, container, itemKey);
            
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
                // cell 在左 → detail 靠右，轴心在左
                pivot.x = 0f;
                localPos.x += cellW * 0.5f;
            }
            else
            {
                // cell 在右 → detail 靠左，轴心在右
                pivot.x = 1f;
                localPos.x -= cellW * 0.5f;
            }
            rt.pivot = pivot;
            rt.anchoredPosition = localPos;
            gameObject.SetActive(true);

        }
    }
}
