using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
/// <summary>
/// 容器 UI 构建 — 生成 Container→Mask→Grid→Cell 完整层级。
/// 不涉及数据，只管拼 UI。
/// </summary>
public static class ContainerBuilder
{
    /// <summary>遍历 specs 数组，逐项构建 + 注册容器</summary>
    public static void BuildAll(Core core)
    {
        foreach (var spec in core.specs)
        {
            var mod = new ContainerMod();
            if (spec.prefab != null)
                BuildFromPrefab(core, spec, mod);
            else
                Build(core, spec, mod);
        }
    }

    static void BuildFromPrefab(Core core, ContainerSpec spec, ContainerMod mod)
    {
        var instance = Object.Instantiate(spec.prefab, core.transform);

        var allChildren = instance.GetComponentsInChildren<RectTransform>(true);
        var list = new System.Collections.Generic.List<RectTransform>();
        foreach (var tr in allChildren)
        {
            if (tr.CompareTag("Cell"))
                list.Add(tr);
        }
        for (int i = 0; i < list.Count; i++)
            list[i].name = i.ToString();

        mod.cells = list.ToArray();
        mod.items = new Item[mod.cells.Length];
        mod.container = instance;
        mod.detail = spec.detail;

        BuildItemUIs(core, mod);
        ContainerManager.containers.Add(mod);
    }

    static void Build(Core core, ContainerSpec spec, ContainerMod mod)
    {
        // Container
        var containerRect = CreateRect("Container", core.transform, typeof(Image));
        containerRect.sizeDelta = new Vector2(
            spec.rows * spec.cellWidth + spec.containerFillHorizontal * 2,
            spec.maskHeight + spec.containerFillUp + spec.containerFillDown);

        // Mask
        var maskRect = CreateRect("Mask", containerRect, typeof(Image), typeof(Mask));
        SetAnchorPivot(maskRect, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
        maskRect.anchoredPosition = new Vector2(0, -spec.containerFillUp);
        maskRect.sizeDelta = new Vector2(spec.rows * spec.cellWidth, spec.maskHeight);

        // Grid
        var gridRect = CreateRect("Grid", maskRect);
        SetAnchorPivot(gridRect, 0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
        gridRect.anchoredPosition = Vector2.zero;
        gridRect.sizeDelta = new Vector2(
            spec.rows * spec.cellWidth,
            Mathf.CeilToInt((float)spec.everyPageTotal / spec.rows) * spec.cellWidth);

        if (spec.totalCells > spec.everyPageTotal)
        {
            // PageText (InputField)
            var pageTextRect = CreateRect("PageText", containerRect, typeof(TMP_InputField));
            SetAnchorPivot(pageTextRect, 0.5f, 0f, 0.5f, 0f, 0.5f, 0f);
            pageTextRect.anchoredPosition = Vector2.zero;
            pageTextRect.sizeDelta = new Vector2(spec.containerFillDown * 6, spec.containerFillDown);

            // Text Area
            var textArea = CreateRect("Text Area", pageTextRect, typeof(RectMask2D));
            textArea.anchorMin = Vector2.zero; textArea.anchorMax = Vector2.one;
            textArea.sizeDelta = Vector2.zero;

            // Text (TextMeshProUGUI)
            var textRect = CreateRect("Text", textArea, typeof(TextMeshProUGUI));
            textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            var tmp = pageTextRect.GetComponent<TMP_InputField>();
            tmp.textViewport = textArea;
            tmp.textComponent = textRect.GetComponent<TextMeshProUGUI>();
            tmp.textComponent.font = core.font;
            tmp.textComponent.fontSize = spec.containerFillDown;
            tmp.textComponent.alignment = TextAlignmentOptions.Center;
            tmp.textComponent.color = Color.white;
            tmp.text = mod.currentPage + "/" + Mathf.CeilToInt((float)spec.totalCells / spec.everyPageTotal);
            // 强制初始化 Caret 内部状态
            tmp.enabled = false;
            tmp.enabled = true;
            

            // PrevButton
            var prevButtonRect = CreateRect("PrevButton", pageTextRect, typeof(Image));
            SetAnchorPivot(prevButtonRect, 0f, 0.5f, 0f, 0.5f, 0f, 0.5f);
            prevButtonRect.anchoredPosition = new Vector2(-spec.containerFillDown, 0);
            prevButtonRect.sizeDelta = new Vector2(spec.containerFillDown, spec.containerFillDown);
            prevButtonRect.gameObject.tag = "TurnPage";

            // NextButton
            var nextButtonRect = CreateRect("NextButton", pageTextRect, typeof(Image));
            SetAnchorPivot(nextButtonRect, 1f, 0.5f, 1f, 0.5f, 1f, 0.5f);
            nextButtonRect.anchoredPosition = new Vector2(spec.containerFillDown, 0);
            nextButtonRect.sizeDelta = new Vector2(spec.containerFillDown, spec.containerFillDown);
            nextButtonRect.gameObject.tag = "TurnPage";
        }

        containerRect.gameObject.tag = "Container";
        gridRect.gameObject.tag = "Grid";
        containerRect.GetComponent<Image>().sprite = spec.containerSprite;
        maskRect.GetComponent<Image>().sprite = spec.maskSprite;

        mod.container = containerRect;
        mod.cells = new RectTransform[spec.everyPageTotal];
        mod.items = new Item[spec.totalCells];
        mod.detail = spec.detail;

        for (int i = 0; i < spec.everyPageTotal; i++)
        {
            var rect = CreateRect("Cell" + i, gridRect, typeof(Image));
            SetAnchorPivot(rect, 0f, 1f, 0f, 1f, 0f, 1f);
            rect.anchoredPosition = new Vector2(
                (i % spec.rows) * spec.cellWidth,
                -(i / spec.rows) * spec.cellWidth);
            rect.sizeDelta = new Vector2(spec.cellWidth, spec.cellWidth);
            rect.gameObject.tag = "Cell";
            rect.GetComponent<Image>().sprite = spec.cellSprite;
            mod.cells[i] = rect;
        }

        BuildItemUIs(core, mod);
        ContainerManager.containers.Add(mod);
    }

    // ─── 内部快捷方法 ───

    /// <summary>遍历 cells[]，为每个 Cell 创建 ItemUI（含 edge + count 子元素）</summary>
    static void BuildItemUIs(Core core, ContainerMod mod)
    {
        mod.itemUIs = new ItemUI[mod.cells.Length];
        for (int i = 0; i < mod.cells.Length; i++)
        {
            var cell = mod.cells[i];

            var itemUIRect = CreateRect("ItemUI", cell, typeof(Image));
            SetAnchorPivot(itemUIRect, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
            itemUIRect.anchoredPosition = Vector2.zero;
            itemUIRect.sizeDelta = cell.sizeDelta * 0.8f;

            var itemImage = itemUIRect.GetComponent<Image>();
            itemImage.raycastTarget = false;

            var edgeRect = CreateRect("edge", cell, typeof(Image));
            SetAnchorPivot(edgeRect, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
            edgeRect.anchoredPosition = Vector2.zero;
            edgeRect.sizeDelta = cell.sizeDelta * 0.8f;

            var countRect = CreateRect("count", cell, typeof(TextMeshProUGUI));
            SetAnchorPivot(countRect, 0.5f, 0f, 0.5f, 0f, 0.5f, 0f);
            countRect.anchoredPosition = Vector2.zero;
            countRect.sizeDelta = new Vector2(cell.sizeDelta.x, cell.sizeDelta.y / 4f);

            var countText = countRect.GetComponent<TextMeshProUGUI>();
            countText.fontSize = 3.9f;
            countText.font = core.font;
            countText.alignment = TextAlignmentOptions.Right;

            mod.itemUIs[i] = new ItemUI
            {
                itemImage = itemImage,
                edge = edgeRect.GetComponent<Image>(),
                count = countText
            };
        }
    }

    /// <summary>创建带 RectTransform 的 GameObject，设父物体，返回 RectTransform</summary>
    static RectTransform CreateRect(string name, Transform parent, params System.Type[] components)
    {
        var types = new System.Type[components.Length + 1];
        types[0] = typeof(RectTransform);
        for (int i = 0; i < components.Length; i++)
            types[i + 1] = components[i];

        var go = new GameObject(name, types);
        go.transform.SetParent(parent, false);
        return go.transform as RectTransform;
    }

    /// <summary>
    /// 一行设 anchorMin + anchorMax + pivot（6 个值）
    /// </summary>
    static void SetAnchorPivot(RectTransform rect,
        float aMinX, float aMinY,
        float aMaxX, float aMaxY,
        float pX, float pY)
    {
        rect.anchorMin = new Vector2(aMinX, aMinY);
        rect.anchorMax = new Vector2(aMaxX, aMaxY);
        rect.pivot = new Vector2(pX, pY);
    }
}
}
