using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 拖拽工具 — 幽灵图标 + 悬停阴影。
    /// 由 Core.Start 初始化，CellHandler 在拖拽时使用。
    /// 每个 Core 持有一个实例，避免多容器场景下的静态状态冲突。
    /// </summary>
    public class DragTool
    {
        public RectTransform dragRect;
        public Image dragItem;
        public Image dragEdge;
        public TextMeshProUGUI dragCount;
        public RectTransform Shadow;

        public void BuildDragItem(Core core)
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

        public void BuildShadow(Core core)
        {
            Shadow = RectUtility.CreateRect("Shadow", core.canvas.transform,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, new(8, 8),
                null,
                typeof(Image));

            var img = Shadow.GetComponent<Image>();
            img.color = core.shadowColor;
            img.raycastTarget = false;

            Shadow.gameObject.SetActive(false);
        }
    }
}
