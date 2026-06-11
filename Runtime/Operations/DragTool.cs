using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 拖拽工具 — 幽灵图标 + 悬停阴影。
    /// 由 Core.Start 初始化，CellHandler 在拖拽时使用。
    /// </summary>
    public static class DragTool
    {
        public static RectTransform dragRect;
        public static Image dragItem;
        public static Image dragEdge;
        public static TextMeshProUGUI dragCount;
        public static RectTransform Shadow;

        public static void BuildDragItem(Core core)
        {
            const float size = 10f;

            dragRect = RectUtility.CreateRect("dragitem", core.canvas.transform,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero,
                new(size, size));

            var itemRect = RectUtility.CreateRect("item", dragRect,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, dragRect.sizeDelta * 0.8f,
                null,
                typeof(Image));
            dragItem = itemRect.GetComponent<Image>();
            dragItem.raycastTarget = false;

            var edgeRect = RectUtility.CreateRect("edge", dragRect,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, dragRect.sizeDelta * 0.8f,
                null,
                typeof(Image));
            dragEdge = edgeRect.GetComponent<Image>();
            dragEdge.raycastTarget = false;

            var countRect = RectUtility.CreateRect("count", dragRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                Vector2.zero,
                new(dragRect.sizeDelta.x, dragRect.sizeDelta.y / 4f),
                null,
                typeof(TextMeshProUGUI));
            dragCount = countRect.GetComponent<TextMeshProUGUI>();
            dragCount.fontSize = core.fontSize;
            dragCount.font = core.font;
            dragCount.alignment = TextAlignmentOptions.Right;
            dragCount.raycastTarget = false;

            dragRect.gameObject.SetActive(false);
        }

        public static void BuildShadow(Core core)
        {
            Shadow = RectUtility.CreateRect("Shadow", core.canvas.transform,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, new(8, 8),
                null,
                typeof(Image));

            var img = Shadow.GetComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.9f);
            img.raycastTarget = false;

            Shadow.gameObject.SetActive(false);
        }
    }
}
