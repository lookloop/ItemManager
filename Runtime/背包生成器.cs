using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
/// <summary>
/// 背包生成器 — 在 UIResponder 下自动构建 背包→Mask→Grid→Cell 完整层级
/// </summary>
public static class 背包生成器
{
    private static Sprite _defaultSprite;

    /// <summary>兜底精灵（1×1 白像素）</summary>
    private static Sprite DefaultSprite
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

    public static void Build(UIResponder _this)
    {
        RectTransform root = _this.transform as RectTransform;

        // ── 1. 背包面板（子物体，不污染根节点） ──
        Transform panelT = root.Find("背包");
        if (panelT == null)
        {
            GameObject panelGo = new GameObject("背包", typeof(RectTransform), typeof(Image));
            panelGo.transform.SetParent(root, false);
            panelGo.tag = "背包";
            panelT = panelGo.transform;
        }
        _this.backpackPanel = panelT as RectTransform;
        Image panelImg = panelT.GetComponent<Image>();
        panelImg.sprite = _this.backpackSprite != null ? _this.backpackSprite : DefaultSprite;
        panelImg.type = Image.Type.Sliced;

        // ── 2. Mask 子物体 ──
        Transform maskT = panelT.Find("Mask");
        if (maskT == null)
        {
            GameObject maskGo = new GameObject("Mask", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
            maskGo.transform.SetParent(panelT, false);
            maskT = maskGo.transform;
        }
        _this.maskTransform = maskT as RectTransform;
        Image maskImg = maskT.GetComponent<Image>();
        maskImg.sprite = _this.maskSprite != null ? _this.maskSprite : DefaultSprite;

        // ── Mask 锚点/轴心 ──
        _this.maskTransform.anchorMin = new Vector2(0.5f, 1f);
        _this.maskTransform.anchorMax = new Vector2(0.5f, 1f);
        _this.maskTransform.pivot = new Vector2(0.5f, 1f);

        // ── 3. Grid 子物体（在 Mask 内） ──
        Transform gridT = maskT.Find("Grid");
        if (gridT == null)
        {
            GameObject gridGo = new GameObject("Grid", typeof(RectTransform));
            gridGo.transform.SetParent(maskT, false);
            gridT = gridGo.transform;
        }
        _this.gridTransform = gridT as RectTransform;
        _this.gridTransform.anchorMin = new Vector2(0.5f, 1f);
        _this.gridTransform.anchorMax = new Vector2(0.5f, 1f);
        _this.gridTransform.pivot = new Vector2(0.5f, 1f);
        _this.gridTransform.anchoredPosition = Vector2.zero;

        // ── 4. 生成 Cell ──
        _this.cellRegistry = new GameObject[_this.cellCount];
        for (int i = 0; i < _this.cellCount; i++)
        {
            GameObject cell = new GameObject(i.ToString(), typeof(RectTransform), typeof(Image));
            cell.transform.SetParent(_this.gridTransform, false);
            RectTransform crt = cell.GetComponent<RectTransform>();
            crt.sizeDelta = new Vector2(_this.cellWidth, _this.cellWidth);
            Image cellImg = cell.GetComponent<Image>();
            cellImg.sprite = _this.cellSprite != null ? _this.cellSprite : DefaultSprite;
            cell.tag = "储物格";
            _this.cellRegistry[i] = cell;
        }
    }
}
}
