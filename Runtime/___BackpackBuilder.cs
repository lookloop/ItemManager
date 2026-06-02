using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
/// <summary>
/// 背包 UI 构建 — 生成 Container→Mask→Grid→Cell 完整层级。
/// 不涉及数据，只管拼 UI。
/// </summary>
public static class BackpackBuilder
{
    static Sprite _defaultSprite;

    static Sprite DefaultSprite
    {
        get
        {
            if (_defaultSprite == null)
            {
                var tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 0.6f));
                tex.Apply();
                _defaultSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            }
            return _defaultSprite;
        }
    }

    /// <summary>构建背包 → 返回顶层 Container GameObject</summary>
    public static GameObject Build(UIResponder _this)
    {
        RectTransform root = _this.transform as RectTransform;

        // 1. Container 面板
        GameObject panelGo = new GameObject("Backpack", typeof(RectTransform), typeof(Image));
        panelGo.transform.SetParent(root, false);
        panelGo.tag = "Container";
        ContainerTouch.backpackPanel = panelGo.transform as RectTransform;
        Image panelImg = panelGo.GetComponent<Image>();
        panelImg.sprite = _this.backpackSprite != null ? _this.backpackSprite : DefaultSprite;
        panelImg.type = Image.Type.Sliced;

        // 2. Mask
        GameObject maskGo = new GameObject("Mask", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
        maskGo.transform.SetParent(panelGo.transform, false);
        ItemTouch.maskTransform = maskGo.transform as RectTransform;
        Image maskImg = maskGo.GetComponent<Image>();
        maskImg.sprite = _this.maskSprite != null ? _this.maskSprite : DefaultSprite;
        ItemTouch.maskTransform.anchorMin = ItemTouch.maskTransform.anchorMax = new Vector2(0.5f, 1f);
        ItemTouch.maskTransform.pivot = new Vector2(0.5f, 1f);

        // 3. Grid
        GameObject gridGo = new GameObject("Grid", typeof(RectTransform));
        gridGo.tag = "Grid";
        gridGo.transform.SetParent(maskGo.transform, false);
        ItemTouch.gridTransform = gridGo.transform as RectTransform;
        ItemTouch.gridTransform.anchorMin = ItemTouch.gridTransform.anchorMax = new Vector2(0.5f, 1f);
        ItemTouch.gridTransform.pivot = new Vector2(0.5f, 1f);
        ItemTouch.gridTransform.anchoredPosition = Vector2.zero;

        // 4. Cell
        ItemTouch.cellRegistry = new GameObject[ItemTouch.cellCount];
        for (int i = 0; i < ItemTouch.cellCount; i++)
        {
            GameObject cell = new GameObject(i.ToString(), typeof(RectTransform), typeof(Image));
            cell.transform.SetParent(ItemTouch.gridTransform, false);
            RectTransform crt = cell.GetComponent<RectTransform>();
            crt.sizeDelta = new Vector2(_this.cellWidth, _this.cellWidth);
            cell.GetComponent<Image>().sprite = _this.cellSprite != null ? _this.cellSprite : DefaultSprite;
            cell.tag = "Item";
            ItemTouch.cellRegistry[i] = cell;
        }

        // 5. 排列 Cell + 设定 Grid/Mask 尺寸
        ApplyCellPositions(_this);
        ApplyGridSize(_this);

        return panelGo;
    }

    static void ApplyCellPositions(UIResponder _this)
    {
        float size = _this.cellWidth;
        float scale = size / 10f;
        for (int i = 0; i < ItemTouch.cellRegistry.Length; i++)
        {
            ItemTouch.cellRegistry[i].name = i.ToString();
            var go = ItemTouch.cellRegistry[i];
            int row = i / ItemTouch.cellsPerRow;
            int col = i % ItemTouch.cellsPerRow;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(col * size, -row * size);
            rt.localScale = new Vector3(scale, scale, 1f);
        }
    }

    static void ApplyGridSize(UIResponder _this)
    {
        float size = _this.cellWidth;
        int totalRows = ItemTouch.cellCount / ItemTouch.cellsPerRow + (ItemTouch.cellCount % ItemTouch.cellsPerRow != 0 ? 1 : 0);
        float gridW = ItemTouch.cellsPerRow * size;
        float gridH = totalRows * size;

        ItemTouch.gridTransform.sizeDelta = new Vector2(gridW, gridH);

        if (ItemTouch.maskTransform != null)
        {
            ItemTouch.maskTransform.sizeDelta = new Vector2(gridW, _this.maskHeight);
            ItemTouch.maskTransform.anchoredPosition = new Vector2(0, _this.maskPosY);
        }

        if (ContainerTouch.backpackPanel != null)
            ContainerTouch.backpackPanel.sizeDelta = new Vector2(
                gridW + _this.horizontalPadding * 2f,
                _this.maskHeight + _this.backpackExtraHeight);
    }
}
}
