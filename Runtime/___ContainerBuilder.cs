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
            ContainerMod containermod = new ContainerMod();
            if (spec.prefab != null)
                BuildFromPrefab(core, spec, containermod);
            else
                Build(core, spec, containermod);
        }
    }

    static void BuildFromPrefab(Core core, ContainerSpec spec, ContainerMod containermod)
    {
        var instance = Object.Instantiate(spec.prefab, core.transform);
        {
            var allChildren = instance.GetComponentsInChildren<RectTransform>(true);
            var list = new System.Collections.Generic.List<RectTransform>();
            foreach (var tr in allChildren)
            {
                if (tr.CompareTag("Cell"))
                    list.Add(tr);
            }
            for (int i = 0; i < list.Count; i++)
                list[i].name = i.ToString();
            containermod.cells = list.ToArray();
            containermod.items = new Item[containermod.cells.Length];
            containermod.container = instance;
            containermod.detail = spec.detail;
            ContainerManager.containers.Add(containermod);
        }
    }

    static void Build(Core core, ContainerSpec spec, ContainerMod containermod)
    {
        // Container（无锚点轴心设置，直接用默认）
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
            // PageText
            var pageTextRect = CreateRect("PageText", containerRect, typeof(TextMeshProUGUI));
            SetAnchorPivot(pageTextRect, 0.5f, 0f, 0.5f, 0f, 0.5f, 0f);
            pageTextRect.anchoredPosition = Vector2.zero;
            pageTextRect.sizeDelta = new Vector2(spec.containerFillDown * 6, spec.containerFillDown);
            pageTextRect.gameObject.tag = "TurnPage";

            var tmp = pageTextRect.GetComponent<TextMeshProUGUI>();
            tmp.text = "1/" + Mathf.CeilToInt((float)spec.totalCells / spec.everyPageTotal);
            tmp.fontSize = spec.containerFillDown;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

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

        containermod.container = containerRect;
        containermod.cells = new RectTransform[spec.everyPageTotal];
        containermod.items = new Item[spec.totalCells];
        containermod.detail = spec.detail;

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
            containermod.cells[i] = rect;
        }

        ContainerManager.containers.Add(containermod);
    }

    // ─── 内部快捷方法 ───

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
    /// 一行设 anchorMin + anchorMax + pivot 三个点（6 个值）
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
