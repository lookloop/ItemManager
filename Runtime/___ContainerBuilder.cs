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
    /// <summary>遍历 specs 数组，逐项构建，containers[i] 对应 specs[i]</summary>
    public static void BuildAll(Core core)
    {
        core.containers = new Container[core.specs.Length];
        for (int i = 0; i < core.specs.Length; i++)
        {
            var mod = new Container();
            core.containers[i] = mod;
            if (core.specs[i].prefab != null)
                BuildFromPrefab(core, core.specs[i], mod);
            else
                Build(core, core.specs[i], mod);
        }
    }

    static void BuildFromPrefab(Core core, ContainerSpec spec, Container mod)
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

        mod.items = new Item[list.Count];
        mod.containerRect = instance;
        mod.detailRect  = spec.detail;
        if (mod.detailRect != null)
            mod.detailFiller = mod.detailRect.GetComponent<IDetailFiller>();

        mod.cells = BuildItemUIs(core, mod, list);
    }

    static void Build(Core core, ContainerSpec spec, Container mod)
    {
        // Container (anchor 默认 0.5,0.5)
        var containerRect = CreateRect("Container", core.transform,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            Vector2.zero,
            new(spec.rows * spec.cellWidth + spec.containerFillHorizontal * 2,
                spec.maskHeight + spec.containerFillUp + spec.containerFillDown),
            "Container",
            typeof(Image));

        // Mask
        var maskRect = CreateRect("Mask", containerRect,
            new(0.5f, 1f), new(0.5f, 1f), new(0.5f, 1f),
            new(0, -spec.containerFillUp),
            new(spec.rows * spec.cellWidth, spec.maskHeight),
            null,
            typeof(Image), typeof(RectMask2D));

        // Grid
        var gridRect = CreateRect("Grid", maskRect,
            new(0.5f, 1f), new(0.5f, 1f), new(0.5f, 1f),
            Vector2.zero,
            new(spec.rows * spec.cellWidth,
                Mathf.CeilToInt((float)spec.everyPageTotal / spec.rows) * spec.cellWidth),
            "Grid");

        if (spec.totalCells > spec.everyPageTotal)
        {
            // PageText (InputField)
            var pageTextRect = CreateRect("PageText", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                Vector2.zero,
                new(spec.pageTextWidth, spec.pageTextHeight),
                null,
                typeof(TMP_InputField));

            // Text Area
            var textArea = CreateRect("Text Area", pageTextRect,
                Vector2.zero, Vector2.one, new(0.5f, 0.5f),
                Vector2.zero, Vector2.zero,
                null,
                typeof(RectMask2D));

            // Text (TextMeshProUGUI)
            var textRect = CreateRect("Text", textArea,
                Vector2.zero, Vector2.one, new(0.5f, 0.5f),
                Vector2.zero, Vector2.zero,
                null,
                typeof(TextMeshProUGUI));

            var tmp = pageTextRect.GetComponent<TMP_InputField>();
            tmp.textViewport = textArea;
            tmp.textComponent = textRect.GetComponent<TextMeshProUGUI>();
            tmp.textComponent.font = core.font;
            tmp.textComponent.fontSize = spec.pageTextHeight;
            tmp.textComponent.alignment = TextAlignmentOptions.Center;
            tmp.textComponent.color = Color.white;
            tmp.text = mod.currentPage + "/" + Mathf.CeilToInt((float)spec.totalCells / spec.everyPageTotal);
            // 强制初始化 Caret 内部状态
            tmp.enabled = false;
            tmp.enabled = true;

            // 聚焦时只显示当前页码（去掉 "/总页数"）
            tmp.onSelect.AddListener(_ =>
            {
                tmp.text = mod.currentPage.ToString();
            });

            // 输入结束：解析 → 钳位 → 翻页 → 恢复显示
            tmp.onEndEdit.AddListener(val =>
            {
                if (int.TryParse(val, out int page))
                {
                    int totalPages = Mathf.CeilToInt((float)spec.totalCells / spec.everyPageTotal);
                    page = Mathf.Clamp(page, 1, totalPages);
                    mod.currentPage = page;
                }
                int total = Mathf.CeilToInt((float)spec.totalCells / spec.everyPageTotal);
                tmp.text = mod.currentPage + "/" + total;

                // 刷新格子
                int start = (mod.currentPage - 1) * mod.cells.Length;
            });
            

            // PrevButton
            var prevButtonRect = CreateRect("PrevButton", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                new(-spec.pageTextWidth / 2 - spec.pageTextHeight / 2, 0),
                new(spec.pageTextHeight, spec.pageTextHeight),
                "TurnPage",
                typeof(Image));

            // NextButton
            var nextButtonRect = CreateRect("NextButton", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                new(spec.pageTextWidth / 2 + spec.pageTextHeight / 2, 0),
                new(spec.pageTextHeight, spec.pageTextHeight),
                "TurnPage",
                typeof(Image));
        }

        containerRect.GetComponent<Image>().sprite = spec.containerSprite;
        maskRect.GetComponent<Image>().sprite = spec.maskSprite;

        mod.containerRect = containerRect;
        mod.maskRect      = maskRect;
        mod.gridRect      = gridRect;
        mod.items = new Item[spec.totalCells];
        if (spec.detail != null)
        {
            mod.detailRect = Object.Instantiate(spec.detail, containerRect);
            mod.detailFiller = mod.detailRect.GetComponent<IDetailFiller>();
        }

        var cellRects = new System.Collections.Generic.List<RectTransform>();
        for (int i = 0; i < spec.everyPageTotal; i++)
        {
            var rect = CreateRect(i.ToString(), gridRect,
                new(0f, 1f), new(0f, 1f), new(0f, 1f),
                new((i % spec.rows) * spec.cellWidth, -(i / spec.rows) * spec.cellWidth),
                new(spec.cellWidth, spec.cellWidth),
                "Cell",
                typeof(Image));
            rect.GetComponent<Image>().sprite = spec.cellSprite;
            cellRects.Add(rect);
        }

        mod.cells = BuildItemUIs(core, mod, cellRects);
    }

    // ─── 内部快捷方法 ───

    /// <summary>遍历 cellRects，为每个 Cell 创建 itemImage + edge + count 子元素，返回 Cell[]</summary>
    static Cell[] BuildItemUIs(Core core, Container mod, System.Collections.Generic.List<RectTransform> cellRects)
    {
        var cells = new Cell[cellRects.Count];
        for (int i = 0; i < cellRects.Count; i++)
        {
            var cellRect = cellRects[i];

            Vector2 halfHalf = new(0.5f, 0.5f);
            var itemSize = cellRect.sizeDelta * 0.8f;

            var itemUIRect = CreateRect("ItemUI", cellRect,
                halfHalf, halfHalf, halfHalf,
                Vector2.zero, itemSize,
                null,
                typeof(Image));
            var itemImage = itemUIRect.GetComponent<Image>();
            itemImage.raycastTarget = false;

            var edgeRect = CreateRect("edge", cellRect,
                halfHalf, halfHalf, halfHalf,
                Vector2.zero, itemSize,
                null,
                typeof(Image));
            edgeRect.GetComponent<Image>().raycastTarget = false;

            var countRect = CreateRect("count", cellRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                Vector2.zero,
                new(cellRect.sizeDelta.x, cellRect.sizeDelta.y / 4f),
                null,
                typeof(TextMeshProUGUI));

            var countText = countRect.GetComponent<TextMeshProUGUI>();
            countText.raycastTarget = false;
            countText.fontSize = 3.9f;
            countText.font = core.font;
            countText.alignment = TextAlignmentOptions.Right;

            cells[i] = new Cell
            {
                cell = cellRect,
                item = itemImage,
                edge = edgeRect.GetComponent<Image>(),
                count = countText
            };

            // 初始隐藏，等 SetViewItem 有数据再显示
            itemImage.gameObject.SetActive(false);
            edgeRect.gameObject.SetActive(false);
            countText.gameObject.SetActive(false);
        }
        return cells;
    }

    /// <summary>
    /// 创建 RectTransform，一步到位：父物体、锚点、pivot、位置、尺寸、tag、组件。
    /// </summary>
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
