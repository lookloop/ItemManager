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
            gameObject.SetActive(true);
        }

        void PositionAtCell(Core core, Container container, int itemKey)
        {
            int cellKey = itemKey - container.cells.Length * (container.currentPage - 1);
            if (cellKey < 0 || cellKey >= container.cells.Length) return;

            RectTransform cellRect = container.cells[cellKey].cell;
            Vector3[] corners = new Vector3[4];
            cellRect.GetWorldCorners(corners);
            Vector3 cellCenter = (corners[0] + corners[2]) * 0.5f;

            RectTransform rt = GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt.parent as RectTransform, cellCenter, core.canvas.worldCamera, out Vector2 localPos);

            float w = rt.sizeDelta.x;
            float cellW = cellRect.sizeDelta.x;
            if (cellCenter.x < Screen.width * 0.5f)
                localPos.x += cellW * 0.5f + w * 0.5f;
            else
                localPos.x -= cellW * 0.5f + w * 0.5f;

            rt.anchoredPosition = localPos;
        }
    }
}
