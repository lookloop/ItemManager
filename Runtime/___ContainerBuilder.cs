using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
/// <summary>
/// 容器 UI 构建 — 生成 Container→Mask→Grid→Cell 完整层级。
/// 同时给各 Rect 挂载对应的交互 Handler（CellHandler / ContainerHandler / TurnPageHandler）。
/// </summary>
public static class ContainerBuilder
{
    public static void BuildAll(Core core)
    {
        core.containers = new Container[core.specs.Length];
        for (int i = 0; i < core.specs.Length; i++)
        {
            var container = new Container();
            core.containers[i] = container;

            if (core.specs[i].prefabRect != null)
                BuildPrefab(core, core.specs[i], container);
            else
                Build(core, core.specs[i], container);

            container.containerRect.name = i.ToString();
        }
    }

    static void BuildPrefab(Core core, ContainerSpec spec, Container container)
    {
        var prefabContainer = Object.Instantiate(spec.prefabRect, core.transform);
        var allRects = prefabContainer.GetComponentsInChildren<RectTransform>(true);
        var cellRects = new List<RectTransform>();

        foreach (var allRect in allRects)
        {
            if (allRect.CompareTag("Cell"))
                cellRects.Add(allRect);
        }

        for (int i = 0; i < cellRects.Count; i++)
            cellRects[i].name = i.ToString();

        container.items = new Item[cellRects.Count];
        container.row = spec.row;
        container.cellWidth = spec.cellWidth;
        container.containerRect = prefabContainer;

        // 给预制体容器挂 ContainerHandler
        var containerHandler = prefabContainer.gameObject.AddComponent<ContainerHandler>();
        containerHandler.core = core;
        containerHandler.container = container;

        if (spec.detailRect != null)
        {
            container.detailRect = Object.Instantiate(spec.detailRect, core.canvas.transform);
            container.detailFiller = container.detailRect.GetComponent<IDetailFiller>();
            container.detailRect.gameObject.SetActive(false);
        }

        container.cells = BuildCellView(core, container, cellRects);
    }

    static void Build(Core core, ContainerSpec spec, Container container)
    {
        var containerRect = RectUtility.CreateRect("Container", core.transform,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            Vector2.zero,
            new(spec.row * spec.cellWidth + spec.containerFillHorizontal * 2,
                spec.maskHeight + spec.containerFillUp + spec.containerFillDown),
            "Container",
            typeof(Image), typeof(RectMask2D));

        var maskRect = RectUtility.CreateRect("Mask", containerRect,
            new(0.5f, 1f), new(0.5f, 1f), new(0.5f, 1f),
            new(0, -spec.containerFillUp),
            new(spec.row * spec.cellWidth, spec.maskHeight),
            null,
            typeof(Image), typeof(RectMask2D));

        var gridRect = RectUtility.CreateRect("Grid", maskRect,
            new(0.5f, 1f), new(0.5f, 1f), new(0.5f, 1f),
            Vector2.zero,
            new(spec.row * spec.cellWidth,
                Mathf.CeilToInt((float)spec.everyPageCells / spec.row) * spec.cellWidth),
            "Grid");

        if (spec.totalItems > spec.everyPageCells)
        {
            var pageTextRect = RectUtility.CreateRect("PageText", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                Vector2.zero,
                new(spec.pageTextWidth, spec.pageTextHeight),
                null,
                typeof(TMP_InputField));

            var textArea = RectUtility.CreateRect("Text Area", pageTextRect,
                Vector2.zero, Vector2.one, new(0.5f, 0.5f),
                Vector2.zero, Vector2.zero,
                null,
                typeof(RectMask2D));

            var textRect = RectUtility.CreateRect("Text", textArea,
                Vector2.zero, Vector2.one, new(0.5f, 0.5f),
                Vector2.zero, Vector2.zero,
                null,
                typeof(TextMeshProUGUI));

            var tmp = pageTextRect.GetComponent<TMP_InputField>();
            container.pageInput = tmp;
            tmp.textViewport = textArea;
            tmp.textComponent = textRect.GetComponent<TextMeshProUGUI>();
            tmp.textComponent.font = core.font;
            tmp.textComponent.fontSize = spec.pageTextHeight;
            tmp.textComponent.alignment = TextAlignmentOptions.Center;
            tmp.textComponent.color = Color.white;
            tmp.text = container.currentPage + "/" + Mathf.CeilToInt((float)spec.totalItems / spec.everyPageCells);
            tmp.enabled = false;
            tmp.enabled = true;

            tmp.onSelect.AddListener(delegate { OnPageInput(tmp, container); });
            tmp.onEndEdit.AddListener(delegate { OffPageInput(core, tmp, container); });

            // ── 翻页按钮 + TurnPageHandler ──
            var prevButtonRect = RectUtility.CreateRect("PrevButton", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                new(-spec.pageTextWidth / 2 - spec.pageTextHeight / 2, 0),
                new(spec.pageTextHeight, spec.pageTextHeight),
                "TurnPage",
                typeof(Image));

            var prevHandler = prevButtonRect.gameObject.AddComponent<TurnPageHandler>();
            prevHandler.core = core;
            prevHandler.container = container;

            var nextButtonRect = RectUtility.CreateRect("NextButton", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                new(spec.pageTextWidth / 2 + spec.pageTextHeight / 2, 0),
                new(spec.pageTextHeight, spec.pageTextHeight),
                "TurnPage",
                typeof(Image));

            var nextHandler = nextButtonRect.gameObject.AddComponent<TurnPageHandler>();
            nextHandler.core = core;
            nextHandler.container = container;
        }

        containerRect.GetComponent<Image>().sprite = spec.containerSprite;
        maskRect.GetComponent<Image>().sprite = spec.maskSprite;
        container.containerRect = containerRect;
        container.maskRect      = maskRect;
        container.gridRect      = gridRect;
        container.items = new Item[spec.totalItems];
        container.row = spec.row;
        container.cellWidth = spec.cellWidth;

        // 给容器挂 ContainerHandler
        var cHandler = containerRect.gameObject.AddComponent<ContainerHandler>();
        cHandler.core = core;
        cHandler.container = container;

        if (spec.detailRect != null)
        {
            container.detailRect = Object.Instantiate(spec.detailRect, core.canvas.transform);
            container.detailFiller = container.detailRect.GetComponent<IDetailFiller>();
            container.detailRect.gameObject.SetActive(false);
        }

        var cellRects = new List<RectTransform>();
        for (int i = 0; i < spec.everyPageCells; i++)
        {
            var rect = RectUtility.CreateRect(i.ToString(), gridRect,
                new(0f, 1f), new(0f, 1f), new(0f, 1f),
                new((i % spec.row) * spec.cellWidth, -(i / spec.row) * spec.cellWidth),
                new(spec.cellWidth, spec.cellWidth),
                "Cell",
                typeof(Image));
            rect.GetComponent<Image>().sprite = spec.cellSprite;
            cellRects.Add(rect);
        }
        container.cells = BuildCellView(core, container, cellRects);
    }

    // ─── Cell 视图 + CellHandler 挂载 ───
    static Cell[] BuildCellView(Core core, Container container, List<RectTransform> cellRects)
    {
        var cells = new Cell[cellRects.Count];
        for (int i = 0; i < cellRects.Count; i++)
        {
            var cellRect = cellRects[i];

            var itemRect = RectUtility.CreateRect("ItemUI", cellRect,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, cellRect.sizeDelta * 0.8f,
                null,
                typeof(Image));
            var itemImage = itemRect.GetComponent<Image>();
            itemImage.raycastTarget = false;

            var edgeRect = RectUtility.CreateRect("edge", cellRect,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, cellRect.sizeDelta * 0.8f,
                null,
                typeof(Image));
            var edgeImage = edgeRect.GetComponent<Image>();
            edgeImage.raycastTarget = false;

            var countRect = RectUtility.CreateRect("count", cellRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                Vector2.zero,
                new(cellRect.sizeDelta.x, cellRect.sizeDelta.y / 4f),
                null,
                typeof(TextMeshProUGUI));
            var countText = countRect.GetComponent<TextMeshProUGUI>();
            countText.fontSize = core.fontSize;
            countText.font = core.font;
            countText.alignment = TextAlignmentOptions.Right;
            countText.raycastTarget = false;

            cells[i] = new Cell
            {
                cell = cellRect,
                item = itemImage,
                edge = edgeImage,
                count = countText
            };

            itemImage.gameObject.SetActive(false);
            edgeImage.gameObject.SetActive(false);
            countText.gameObject.SetActive(false);

            // ── 挂载 CellHandler ──
            var handler = cellRect.gameObject.AddComponent<CellHandler>();
            handler.core = core;
            handler.container = container;
            handler.cellKey = i;
        }
        return cells;
    }

    // ─── 页码输入 ───
    static void OnPageInput(TMP_InputField tmp, Container container)
    {
        tmp.text = container.currentPage.ToString();
    }

    static void OffPageInput(Core core, TMP_InputField tmp, Container container)
    {
        if (int.TryParse(tmp.text, out int page))
        {
            SetPage.Set(core, container, page);
        }
    }
}
}
