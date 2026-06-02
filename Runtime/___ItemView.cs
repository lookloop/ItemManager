using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
/// <summary>
/// Item 视觉 — 数据 → 画面同步。
/// 负责创建/更新/清空 Cell 内的 Item 显示（图标 + 光晕 + 数量文字）。
/// </summary>
public static class ItemView
{
    /// <summary>清空旧 Item → 创建新 Item → 加载 Addressables 资源 → 显示</summary>
    public static async void Sync(UIResponder _this, int index)
    {
        var cell     = ItemTouch.cellRegistry[index];
        var itemData = ContainerManager.containers[0].items[index];

        var oldItem = cell.transform.Find("Item");
        if (oldItem != null) Object.Destroy(oldItem.gameObject);

        if (itemData == null || itemData.Id <= 0) return;

        var itemInstance = Create(_this, cell);
        var table = await _this.GetItemTable(itemData.Id.ToString());

        if (cell == null || _this == null || ItemTouch.cellRegistry == null)
        {
            if (itemInstance != null) Object.Destroy(itemInstance);
            return;
        }
        if (table == null) return;

        var body = itemInstance.GetComponent<Image>();
        if (table.ItemSprite != null)
        {
            body.sprite = table.ItemSprite;
            body.enabled = true;
        }

        if (itemInstance.transform.childCount > 0)
        {
            var glow = itemInstance.transform.GetChild(0);
            if (table.GlowSprite != null)
            {
                glow.GetComponent<Image>().sprite = table.GlowSprite;
                glow.gameObject.SetActive(true);
            }
        }

        if (itemInstance.transform.childCount > 1)
            itemInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemData.Count.ToString();
    }

    static GameObject Create(UIResponder _this, GameObject cell)
    {
        var m = ContainerManager.containers[0].mod;
        var iw = m != null ? m.itemWidth : 8f;
        var go = new GameObject("Item", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(cell.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(iw, iw);
        rt.anchoredPosition = Vector2.zero;
        var img = go.GetComponent<Image>();
        img.enabled = false;
        img.raycastTarget = false;

        // Glow
        var glow = new GameObject("Glow", typeof(RectTransform), typeof(Image));
        glow.transform.SetParent(go.transform, false);
        var grt = glow.GetComponent<RectTransform>();
        grt.anchorMin = Vector2.zero; grt.anchorMax = Vector2.one; grt.sizeDelta = Vector2.zero;
        glow.SetActive(false);
        glow.GetComponent<Image>().raycastTarget = false;

        // Count
        var count = new GameObject("Count", typeof(RectTransform), typeof(TextMeshProUGUI));
        count.transform.SetParent(go.transform, false);
        var crt = count.GetComponent<RectTransform>();
        crt.anchorMin = crt.anchorMax = new Vector2(1, 0);
        crt.pivot = new Vector2(1, 0); crt.sizeDelta = new Vector2(10, 5);
        var tmp = count.GetComponent<TextMeshProUGUI>();
        if (m != null && m.itemFont != null) tmp.font = m.itemFont;
        tmp.fontSize = 3.9f;
        tmp.alignment = TextAlignmentOptions.BottomRight;
        tmp.raycastTarget = false;

        return go;
    }
}
}
