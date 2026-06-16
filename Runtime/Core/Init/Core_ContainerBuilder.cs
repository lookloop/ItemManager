using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
public partial class Core
{
    void BuildAll()
    {
        containers = new Container[specs.Length];
        for (int i = 0; i < specs.Length; i++)
        {
            var container = new Container();
            container.itemFilter = specs[i].itemFilter;
            container.containerIndex = i;
            containers[i] = container;

            if (specs[i].prefabRect != null)
                BuildPrefab(specs[i], container);
            else
                Build(specs[i], container);

            container.containerRect.name = i.ToString();
        }
    }

    void BuildPrefab(ContainerSpec spec, Container container)
    {
        var prefabContainer = Instantiate(spec.prefabRect, transform);
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
        container.cellWidth = cellSize;
        container.containerRect = prefabContainer;

        AttachContainerTouch(container, prefabContainer.gameObject);
        AttachDetail(container, spec);
        container.cells = BuildCellView(container, cellRects);
    }

    void Build(ContainerSpec spec, Container container)
    {
        var containerRect = CreateRect("Container", transform,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            Vector2.zero,
            new(spec.row * cellSize + spec.containerFillHorizontal * 2,
                spec.maskHeight + spec.containerFillUp + spec.containerFillDown),
            "Container",
            typeof(Image), typeof(RectMask2D));

        var maskRect = CreateRect("Mask", containerRect,
            new(0.5f, 1f), new(0.5f, 1f), new(0.5f, 1f),
            new(0, -spec.containerFillUp),
            new(spec.row * cellSize, spec.maskHeight),
            null,
            typeof(Image), typeof(RectMask2D));

        var gridRect = CreateRect("Grid", maskRect,
            new(0.5f, 1f), new(0.5f, 1f), new(0.5f, 1f),
            Vector2.zero,
            new(spec.row * cellSize,
                Mathf.CeilToInt((float)spec.everyPageCells / spec.row) * cellSize),
            "Grid");

        if (spec.totalItems > spec.everyPageCells)
        {
            var pageTextRect = CreateRect("PageText", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                Vector2.zero,
                new(spec.pageTextWidth, spec.pageTextHeight),
                null,
                typeof(TMP_InputField));

            var textArea = CreateRect("Text Area", pageTextRect,
                Vector2.zero, Vector2.one, new(0.5f, 0.5f),
                Vector2.zero, Vector2.zero,
                null,
                typeof(RectMask2D));

            var textRect = CreateRect("Text", textArea,
                Vector2.zero, Vector2.one, new(0.5f, 0.5f),
                Vector2.zero, Vector2.zero,
                null,
                typeof(TextMeshProUGUI));

            var tmp = pageTextRect.GetComponent<TMP_InputField>();
            container.pageInput = tmp;
            tmp.textViewport = textArea;
            tmp.textComponent = textRect.GetComponent<TextMeshProUGUI>();
            tmp.textComponent.font = font;
            tmp.textComponent.fontSize = spec.pageTextHeight;
            tmp.textComponent.alignment = TextAlignmentOptions.Center;
            tmp.textComponent.color = Color.white;
            tmp.text = container.currentPage + "/" + Mathf.CeilToInt((float)spec.totalItems / spec.everyPageCells);
            // enabled 开关强制刷新 TMP_InputField 文本显示
            tmp.enabled = false;
            tmp.enabled = true;

            tmp.onSelect.AddListener(delegate { OnPageInput(tmp, container); });
            tmp.onEndEdit.AddListener(delegate { OffPageInput(tmp, container); });

            // ── 翻页按钮 + TurnPageHandler ──
            var prevButtonRect = CreateRect("PrevButton", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                new(-spec.pageTextWidth / 2 - spec.pageTextHeight / 2, 0),
                new(spec.pageTextHeight, spec.pageTextHeight),
                "TurnPage",
                typeof(Image));

            if (turnPageSprite != null)
            {
                prevButtonRect.GetComponent<Image>().sprite = turnPageSprite;
                prevButtonRect.localRotation = Quaternion.Euler(0, 0, 180f);
            }

            var prevHandler = prevButtonRect.gameObject.AddComponent<TurnPageTouch>();
            prevHandler.core = this;
            prevHandler.container = container;
            prevHandler.direction = -1;

            var nextButtonRect = CreateRect("NextButton", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                new(spec.pageTextWidth / 2 + spec.pageTextHeight / 2, 0),
                new(spec.pageTextHeight, spec.pageTextHeight),
                "TurnPage",
                typeof(Image));

            if (turnPageSprite != null)
                nextButtonRect.GetComponent<Image>().sprite = turnPageSprite;

            var nextHandler = nextButtonRect.gameObject.AddComponent<TurnPageTouch>();
            nextHandler.core = this;
            nextHandler.container = container;
            nextHandler.direction = 1;
        }

        containerRect.GetComponent<Image>().sprite = spec.containerSprite;
        maskRect.GetComponent<Image>().sprite = spec.maskSprite;
        container.containerRect = containerRect;
        container.maskRect      = maskRect;
        container.gridRect      = gridRect;
        container.items = new Item[spec.totalItems];
        container.row = spec.row;
        container.cellWidth = cellSize;

        AttachContainerTouch(container, containerRect.gameObject);
        AttachDetail(container, spec);

        var cellRects = new List<RectTransform>();
        for (int i = 0; i < spec.everyPageCells; i++)
        {
            var rect = CreateRect(i.ToString(), gridRect,
                new(0f, 1f), new(0f, 1f), new(0f, 1f),
                new((i % spec.row) * cellSize, -(i / spec.row) * cellSize),
                new(cellSize, cellSize),
                "Cell",
                typeof(Image));
            rect.GetComponent<Image>().sprite = spec.cellSprite;
            cellRects.Add(rect);
        }
        container.cells = BuildCellView(container, cellRects);
    }

    // ─── Cell 视图 + CellHandler 挂载 ───
    Cell[] BuildCellView(Container container, List<RectTransform> cellRects)
    {
        var cells = new Cell[cellRects.Count];
        for (int i = 0; i < cellRects.Count; i++)
        {
            var cellRect = cellRects[i];

            var itemRect = CreateRect("ItemUI", cellRect,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, cellRect.sizeDelta * 0.8f,
                null,
                typeof(Image));
            var itemImage = itemRect.GetComponent<Image>();
            itemImage.raycastTarget = false;

            var edgeRect = CreateRect("edge", cellRect,
                new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
                Vector2.zero, cellRect.sizeDelta * 0.8f,
                null,
                typeof(Image));
            var edgeImage = edgeRect.GetComponent<Image>();
            edgeImage.raycastTarget = false;

            var countRect = CreateRect("count", cellRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                Vector2.zero,
                new(cellRect.sizeDelta.x, cellRect.sizeDelta.y / 4f),
                null,
                typeof(TextMeshProUGUI));
            var countText = countRect.GetComponent<TextMeshProUGUI>();
            countText.fontSize = fontSize;
            countText.font = font;
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
            var handler = cellRect.gameObject.AddComponent<CellTouch>();
            handler.core = this;
            handler.container = container;
            handler.cellKey = i;
        }
        return cells;
    }

    // ─── Handler 挂载辅助 ───
    void AttachContainerTouch(Container container, GameObject go)
    {
        var handler = go.AddComponent<ContainerTouch>();
        handler.core = this;
        handler.container = container;
    }

    void AttachDetail(Container container, ContainerSpec spec)
    {
        if (spec.detailRect == null) return;
        container.detailRect = Instantiate(spec.detailRect, canvas.transform);
        container.detailFiller = container.detailRect.GetComponent<DetailBase>();
        container.detailRect.gameObject.SetActive(false);
    }

    // ─── 页码输入 ───
    static void OnPageInput(TMP_InputField tmp, Container container)
    {
        tmp.text = container.currentPage.ToString();
    }

    void OffPageInput(TMP_InputField tmp, Container container)
    {
        if (int.TryParse(tmp.text, out int page))
            SetPage(container, page);
    }
}
}
