using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// OtherTool — 通用工具方法，供 TouchCell、TouchItem 等模块调用。
    /// </summary>
    public static class OtherTool
    {
        /// <summary>
        /// 拖拽显示的幽灵 Cell — 挂在 Canvas 下，跟随手指。
        /// </summary>
        public static RectTransform dragRect;
        public static Image dragItem;
        public static Image dragEdge;
        public static TextMeshProUGUI dragCount;
        public static RectTransform Shadow;

        /// <summary>
        /// 在 Canvas 下构建一个幽灵 Cell，结构和普通 Cell 一致，用于拖拽显示。
        /// </summary>
        public static void BuildDragItem(Core core)
        {
            const float size = 10f;

            dragRect = CreateRect("dragitem", core.canvas.transform,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero,
                new(size, size));

            // 三个子对象，和 ContainerBuilder.BuildCellView 里一致
            var itemRect = CreateRect("item", dragRect,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, dragRect.sizeDelta * 0.8f,
                null,
                typeof(Image));
            dragItem = itemRect.GetComponent<Image>();
            dragItem.raycastTarget = false;

            var edgeRect = CreateRect("edge", dragRect,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, dragRect.sizeDelta * 0.8f,
                null,
                typeof(Image));
            dragEdge = edgeRect.GetComponent<Image>();
            dragEdge.raycastTarget = false;

            var countRect = CreateRect("count", dragRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                Vector2.zero,
                new(dragRect.sizeDelta.x, dragRect.sizeDelta.y / 4f),
                null,
                typeof(TextMeshProUGUI));
            dragCount = countRect.GetComponent<TextMeshProUGUI>();
            dragCount.fontSize = 3.9f;
            dragCount.font = core.font;
            dragCount.alignment = TextAlignmentOptions.Right;
            dragCount.raycastTarget = false;

            dragRect.gameObject.SetActive(false);
        }

        /// <summary>
        /// 悬停阴影 — 灰色半透明，拖拽时插入目标 Cell 下方作为占位提示。
        /// </summary>

        public static void BuildShadow(Core core)
        {
            Shadow = CreateRect("Shadow", core.canvas.transform,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, new(8, 8),
                null,
                typeof(Image));

            var img = Shadow.GetComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.9f);
            img.raycastTarget = false;

            Shadow.gameObject.SetActive(false);
        }

        static RectTransform CreateRect(string name, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPosition, Vector2 sizeDelta,
            string tag = null,
            params System.Type[] components)
        {
            var types = new System.Type[components.Length + 1];
            types[0] = typeof(RectTransform);
            for (int i = 0; i < components.Length; i++)
                types[i + 1] = components[i];

            var go = new GameObject(name, types);
            var rect = go.transform as RectTransform;
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            if (tag != null) go.tag = tag;
            return rect;
        }
    }
}
