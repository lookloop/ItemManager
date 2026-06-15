using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class Core
{
    void BuildDragTool()
    {
        dragParent = CreateRect("dragitem", canvas.transform,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            Vector2.zero,
            new(cellSize, cellSize));

        var itemRect = CreateRect("item", dragParent,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            Vector2.zero, dragParent.sizeDelta * 0.8f,
            null,
            typeof(Image));
        dragItem = itemRect.GetComponent<Image>();
        dragItem.raycastTarget = false;

        var edgeRect = CreateRect("edge", dragParent,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            Vector2.zero, dragParent.sizeDelta * 0.8f,
            null,
            typeof(Image));
        dragEdge = edgeRect.GetComponent<Image>();
        dragEdge.raycastTarget = false;

        var countRect = CreateRect("count", dragParent,
            new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
            Vector2.zero,
            new(dragParent.sizeDelta.x, dragParent.sizeDelta.y / 4f),
            null,
            typeof(TextMeshProUGUI));
        dragCount = countRect.GetComponent<TextMeshProUGUI>();
        dragCount.fontSize = fontSize;
        dragCount.font = font;
        dragCount.alignment = TextAlignmentOptions.Right;
        dragCount.raycastTarget = false;

        dragParent.gameObject.SetActive(false);

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

        // ── Debug Text ──
        var tmpTipRect = CreateRect("tmpTip", canvas.transform,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            new(10f, -10f),
            new(cellSize * 6f, cellSize * 4f),
            null,
            typeof(TextMeshProUGUI));
        tmpTip = tmpTipRect.GetComponent<TextMeshProUGUI>();
        tmpTip.fontSize = fontSize;
        tmpTip.font = font;
        tmpTip.raycastTarget = false;
    }
}
}
