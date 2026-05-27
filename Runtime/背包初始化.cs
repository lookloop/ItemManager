using UnityEngine;

public static class 背包初始化
{
    public static void Execute(UIResponder _this)
    {
        _this.items = new Item[_this.cellCount];
        ApplyCellPositions(_this);
        ApplyGridSize(_this);

        for (int i = 0; i < _this.cellCount; i++)
        {
            if (Random.Range(0, 2) == 1)
            {
                int type = Random.Range(1, 5);
                设置格子(_this, i, new Item { Id = type, Type = type });
            }
            else
            {
                设置格子(_this, i, null);
            }
        }
    }

    /// <summary>写入数据并同步画面（唯一入口，外部只调这个方法）</summary>
    public static void 设置格子(UIResponder _this, int index, Item item)
    {
        _this.items[index] = item;
        同步格子(_this, index);
    }

    /// <summary>数组 → 视觉同步：有数据则创建/更新 Item，无数据则清空</summary>
    private static async void 同步格子(UIResponder _this, int index)
    {
        var cell = _this.cellRegistry[index];
        var itemData = _this.items[index];

        // 清空旧 Item
        var oldItem = cell.transform.Find("Item");
        if (oldItem != null) Object.Destroy(oldItem.gameObject);

        if (itemData == null || itemData.Id <= 0) return;

        // 创建 Item
        var itemInstance = 创建Item(_this, cell);

        // 加载 ItemTable 并填充
        var table = await _this.GetItemTable(itemData.Id.ToString());
        if (table == null) return;

        var body = itemInstance.GetComponent<UnityEngine.UI.Image>();
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
                glow.GetComponent<UnityEngine.UI.Image>().sprite = table.GlowSprite;
                glow.gameObject.SetActive(true);
            }
        }

        if (itemInstance.transform.childCount > 1)
            itemInstance.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = itemData.Count.ToString();
    }

    private static GameObject 创建Item(UIResponder _this, GameObject cell)
    {
        var iw = _this.itemWidth;
        var go = new GameObject("Item", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        go.transform.SetParent(cell.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(iw, iw);
        rt.anchoredPosition = Vector2.zero;
        var img = go.GetComponent<UnityEngine.UI.Image>();
        img.enabled = false;
        img.raycastTarget = false;

        // Glow
        var glow = new GameObject("Glow", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        glow.transform.SetParent(go.transform, false);
        var grt = glow.GetComponent<RectTransform>();
        grt.anchorMin = Vector2.zero; grt.anchorMax = Vector2.one; grt.sizeDelta = Vector2.zero;
        glow.SetActive(false);
        glow.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;

        // Count
        var count = new GameObject("Count", typeof(RectTransform), typeof(TMPro.TextMeshProUGUI));
        count.transform.SetParent(go.transform, false);
        var crt = count.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(1, 0); crt.anchorMax = new Vector2(1, 0);
        crt.pivot = new Vector2(1, 0); crt.sizeDelta = new Vector2(10, 5);
        var tmp = count.GetComponent<TMPro.TextMeshProUGUI>();
        if (_this.itemFont != null) tmp.font = _this.itemFont;
        tmp.fontSize = 3.9f;
        tmp.alignment = TMPro.TextAlignmentOptions.BottomRight;
        tmp.raycastTarget = false;

        return go;
    }

    private static void ApplyCellPositions(UIResponder _this)
    {
        float size = _this.cellWidth;
        float scale = size / 10f;
        for (int i = 0; i < _this.cellRegistry.Length; i++)
        {
            _this.cellRegistry[i].name = i.ToString();
            var go = _this.cellRegistry[i];
            int row = i / _this.cellsPerRow;
            int col = i % _this.cellsPerRow;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(col * size, -row * size);
            rt.localScale = new Vector3(scale, scale, 1f);
        }
    }

    private static void ApplyGridSize(UIResponder _this)
    {
        float size = _this.cellWidth;
        int totalRows = _this.cellCount / _this.cellsPerRow + (_this.cellCount % _this.cellsPerRow != 0 ? 1 : 0);
        float gridW = _this.cellsPerRow * size;
        float gridH = totalRows * size;

        _this.gridTransform.sizeDelta = new Vector2(gridW, gridH);

        if (_this.maskTransform != null)
        {
            _this.maskTransform.sizeDelta = new Vector2(gridW, _this.maskHeight);
            _this.maskTransform.anchoredPosition = new Vector2(0, _this.maskPosY);
        }

        if (_this.backpackPanel != null)
            _this.backpackPanel.sizeDelta = new Vector2(
                gridW + _this.horizontalPadding * 2f,
                _this.maskHeight + _this.backpackExtraHeight);
    }
}
