using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{

public static class MiscInit
{
    public static RectTransform parent;
    public static Image itemImage;
    public static Image edge;
    public static TextMeshProUGUI count;

    /// <summary>内部生成 parent(10×10) + ItemUI 三兄弟(img8×8, img8×8, tmp)</summary>
    public static void CreateTemporaryItemUI(Canvas canvas)
    {
        // parent — 10×10，挂在 canvas 下
        parent = new GameObject("ItemUIParent", typeof(RectTransform)).GetComponent<RectTransform>();
        parent.SetParent(canvas.transform, false);
        parent.sizeDelta = new Vector2(10f, 10f);

        // itemImage — 8×8 居中
        var itemUIGo = new GameObject("ItemUI", typeof(RectTransform), typeof(Image));
        var itemRect = itemUIGo.transform as RectTransform;
        itemRect.SetParent(parent, false);
        itemRect.anchorMin = itemRect.anchorMax = itemRect.pivot = new Vector2(0.5f, 0.5f);
        itemRect.anchoredPosition = Vector2.zero;
        itemRect.sizeDelta = new Vector2(8f, 8f);
        itemImage = itemUIGo.GetComponent<Image>();
        itemImage.raycastTarget = false;
        itemUIGo.SetActive(false);

        // edge — 8×8 居中
        var edgeGo = new GameObject("edge", typeof(RectTransform), typeof(Image));
        var edgeRect = edgeGo.transform as RectTransform;
        edgeRect.SetParent(parent, false);
        edgeRect.anchorMin = edgeRect.anchorMax = edgeRect.pivot = new Vector2(0.5f, 0.5f);
        edgeRect.anchoredPosition = Vector2.zero;
        edgeRect.sizeDelta = new Vector2(8f, 8f);
        edge = edgeGo.GetComponent<Image>();
        edge.raycastTarget = false;
        edgeGo.SetActive(false);

        // count — 底部居中 (参考 Builder)
        var countGo = new GameObject("count", typeof(RectTransform), typeof(TextMeshProUGUI));
        var countRect = countGo.transform as RectTransform;
        countRect.SetParent(parent, false);
        countRect.anchorMin = countRect.anchorMax = countRect.pivot = new Vector2(0.5f, 0f);
        countRect.anchoredPosition = Vector2.zero;
        countRect.sizeDelta = new Vector2(10f, 2.5f);
        count = countGo.GetComponent<TextMeshProUGUI>();
        count.raycastTarget = false;
        count.fontSize = 3.9f;
        count.alignment = TextAlignmentOptions.Right;
        countGo.SetActive(false);

        
    }
}

}
