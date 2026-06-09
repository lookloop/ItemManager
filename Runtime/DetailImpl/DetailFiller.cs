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
            // 1. 定位 cell
            int cellKey = itemKey - container.cells.Length * (container.currentPage - 1);
            if (cellKey < 0 || cellKey >= container.cells.Length) return;
            RectTransform cellRect = container.cells[cellKey].cell;

            // 2. cell 中心 → Canvas 局部坐标
            Vector3[] corners = new Vector3[4];
            cellRect.GetWorldCorners(corners);
            Vector3 cellCenter = (corners[0] + corners[2]) * 0.5f;

            RectTransform rt = GetComponent<RectTransform>();
            RectTransform parentRt = rt.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRt, cellCenter, core.canvas.worldCamera, out Vector2 localPos);

            // 3. 左右判断，让 detail 不遮挡 cell
            float w = rt.sizeDelta.x;
            float cellW = cellRect.sizeDelta.x;
            if (cellCenter.x < Screen.width * 0.5f)
                localPos.x += cellW * 0.5f + w * 0.5f;  // cell 在左 → detail 靠右
            else
                localPos.x -= cellW * 0.5f + w * 0.5f;  // cell 在右 → detail 靠左

            rt.anchoredPosition = localPos;

            // 4. 显示
            gameObject.SetActive(true);
        }
    }
}
