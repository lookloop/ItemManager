using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class Core
{
    void BuildDragTool()
    {
        dragRect = CreateRect("dragitem", canvas.transform,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            Vector2.zero,
            new(cellSize, cellSize));

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
        dragCount.fontSize = fontSize;
        dragCount.font = font;
        dragCount.alignment = TextAlignmentOptions.Right;
        dragCount.raycastTarget = false;

        dragRect.gameObject.SetActive(false);

        // ── Shadow ──
        Shadow = CreateRect("Shadow", canvas.transform,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            Vector2.zero, new(cellSize * 0.8f, cellSize * 0.8f),
            null,
            typeof(Image));

        var img = Shadow.GetComponent<Image>();
        img.color = shadowColor;
        img.raycastTarget = false;

        Shadow.gameObject.SetActive(false);
    }
}
}
