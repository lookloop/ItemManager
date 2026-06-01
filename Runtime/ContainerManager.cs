using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
/// <summary>
/// Container 数据管理器 — 注册容器、增删改查格子数据。
/// 不涉及 UI 构建，只管数据 ↔ 视觉同步。
/// </summary>
public static class ContainerManager
{
    /// <summary>
    /// 注册容器对象 → 分配 ID，初始化 items 数组，写入 containers 列表。
    /// 返回容器 ID（索引）。
    /// </summary>
    public static int 注册(GameObject containerObj, UIResponder _this)
    {
        if (_this.containers == null)
            _this.containers = new System.Collections.Generic.List<ContainerData>();

        var cd = new ContainerData
        {
            container = containerObj.transform as RectTransform,
            items     = new Item[_this.cellCount]
        };

        _this.containers.Add(cd);
        _this.items = cd.items; // 向后兼容快捷引用

        return _this.containers.Count - 1;
    }

    /// <summary>写入数据并同步画面</summary>
    public static void 设置格子(UIResponder _this, int index, Item item)
    {
        if (_this.containers != null && _this.containers.Count > 0)
            _this.containers[0].items[index] = item;
        _this.items[index] = item;
        同步格子(_this, index);
    }

    /// <summary>数组 → 视觉同步</summary>
    static async void 同步格子(UIResponder _this, int index)
    {
        var cell     = _this.cellRegistry[index];
        var itemData = _this.items[index];

        var oldItem = cell.transform.Find("Item");
        if (oldItem != null) Object.Destroy(oldItem.gameObject);

        if (itemData == null || itemData.Id <= 0) return;

        var itemInstance = 创建Item(_this, cell);
        var table = await _this.GetItemTable(itemData.Id.ToString());

        if (cell == null || _this == null || _this.cellRegistry == null)
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

    static GameObject 创建Item(UIResponder _this, GameObject cell)
    {
        var iw = _this.itemWidth;
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
        if (_this.itemFont != null) tmp.font = _this.itemFont;
        tmp.fontSize = 3.9f;
        tmp.alignment = TextAlignmentOptions.BottomRight;
        tmp.raycastTarget = false;

        return go;
    }
}
}
