using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
/// <summary>
/// 容器 UI 构建 — 生成 Container→Mask→Grid→Cell 完整层级。
/// 不涉及数据，只管拼 UI。
/// </summary>
public static class ContainerBuilder
{
    /// <summary>遍历 mods 数组，逐项构建 + 注册容器</summary>
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
    public static void BuildFromPrefab(Core core, ContainerSpec spec, ContainerMod containermod)
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
            //detail存入mod
            containermod.detail = spec.detail;
            ContainerManager.containers.Add(containermod);
        }
    }
    public static void Build(Core core, ContainerSpec spec, ContainerMod containermod)
    {
        // Container新建，设置父物体为core，设置tag，设置RectTransform大小，设置Image组件的sprite，存入mod
        var container = new GameObject("Container", typeof(RectTransform), typeof(Image));
        container.transform.SetParent(core.transform, false);
        var containerRect = container.GetComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(spec.rows * spec.cellWidth + spec.containerFillHorizontal * 2, spec.maskHeight + spec.containerFillUp + spec.containerFillDown);
        //新建Mask，设置父物体为Container，设置RectTransform大小和位置，设置Image组件的sprite，添加Mask组件。不设置tag，不存入mod
        var mask = new GameObject("Mask", typeof(RectTransform), typeof(Image), typeof(Mask));
        mask.transform.SetParent(container.transform, false);
        var maskRect = mask.GetComponent<RectTransform>();
        maskRect.sizeDelta = new Vector2(spec.rows * spec.cellWidth, spec.maskHeight);
        maskRect.anchorMin = new Vector2(0.5f, 1);
        maskRect.anchorMax = new Vector2(0.5f, 1);
        maskRect.pivot = new Vector2(0.5f, 1);
        maskRect.anchoredPosition = new Vector2(0, -spec.containerFillUp);
        //新建Grid，设置父物体为Mask，设置tag，设置RectTransform大小和位置，不设置Image组件，不存入mod
        var grid = new GameObject("Grid", typeof(RectTransform));
        grid.transform.SetParent(mask.transform, false);
        var gridRect = grid.GetComponent<RectTransform>();
        gridRect.sizeDelta = new Vector2(spec.rows * spec.cellWidth, Mathf.CeilToInt((float)spec.everyPageTotal / spec.rows) * spec.cellWidth);
        gridRect.anchorMin = new Vector2(0.5f, 1);
        gridRect.anchorMax = new Vector2(0.5f, 1);
        gridRect.pivot = new Vector2(0.5f, 1);
        gridRect.anchoredPosition = Vector2.zero;
        

        container.tag = "Container";
        grid.tag = "Grid";
        container.GetComponent<Image>().sprite = spec.containerSprite;
        mask.GetComponent<Image>().sprite = spec.maskSprite;
        containermod.container = containerRect;
        containermod.cells = new RectTransform[spec.everyPageTotal];
        containermod.items = new Item[spec.totalCells];
        containermod.detail = spec.detail;



        for (int i = 0; i < spec.everyPageTotal; i++)
        {
            var cell = new GameObject("Cell" + i, typeof(RectTransform), typeof(Image));
            cell.transform.SetParent(grid.transform, false);
            var rect = cell.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(spec.cellWidth, spec.cellWidth);
            rect.anchoredPosition = new Vector2((i % spec.rows) * spec.cellWidth, -(i / spec.rows) * spec.cellWidth);
            cell.tag = "Cell";
            cell.GetComponent<Image>().sprite = spec.cellSprite;
            containermod.cells[i] = rect;
        }
        ContainerManager.containers.Add(containermod);
    }
}
}