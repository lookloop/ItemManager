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
        //新建一个对象，命名为 Container
        var container = new GameObject("Container", typeof(RectTransform), typeof(Image));
        container.transform.SetParent(core.transform);
        //设置tag为Container
        container.tag = "Container";
        //新建一个对象,命名为 Mask，设置父对象为 Container
        var mask = new GameObject("Mask", typeof(RectTransform), typeof(Image), typeof(Mask));
        mask.transform.SetParent(container.transform);
        //新建一个对象,命名为 Grid，设置父对象为 Mask
        var grid = new GameObject("Grid", typeof(RectTransform));
        grid.transform.SetParent(mask.transform);
        //设置标签为Grid
        grid.tag = "Grid";
        //根据每一页的格子数量进行生成
        var gridRect = grid.GetComponent<RectTransform>();
        gridRect.sizeDelta = new Vector2(spec.rows * spec.cellWidth, Mathf.CeilToInt((float)spec.everyPageTotal / spec.rows) * spec.cellWidth);
        var maskRect = mask.GetComponent<RectTransform>();
        maskRect.sizeDelta = new Vector2(spec.rows * spec.cellWidth, spec.maskHeight);
        //设置container高度宽度，宽度为mask宽度+水平内边距*2，高度为mask高度+
        var containerRect = container.GetComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(maskRect.sizeDelta.x + spec.containerFillHorizontal * 2, maskRect.sizeDelta.y + spec.containerFillUp + spec.containerFillDown);
        maskRect.anchorMin = new Vector2(0.5f, 1);
        maskRect.anchorMax = new Vector2(0.5f, 1);
        maskRect.pivot = new Vector2(0.5f, 1);
        //设置mask位置，x=0，y=-面板上边距
        maskRect.anchoredPosition = new Vector2(0, -spec.containerFillUp);
        //设置grid的轴心和锚点，和mask一样
        gridRect.anchorMin = new Vector2(0.5f, 1);
        gridRect.anchorMax = new Vector2(0.5f, 1);
        gridRect.pivot = new Vector2(0.5f, 1);
        gridRect.anchoredPosition = Vector2.zero;
        //
        //
        //给containerrect的图片精灵改了
        container.GetComponent<Image>().sprite = spec.containerSprite;
        //给mask的图片精灵改了
        mask.GetComponent<Image>().sprite = spec.maskSprite;
        containermod.container = containerRect;
        //将cells列表存入mod
        containermod.cells = new RectTransform[spec.everyPageTotal];
        containermod.items = new Item[spec.totalCells];
        containermod.detail = spec.detail;



        for (int i = 0; i < spec.everyPageTotal; i++)
        {
            //新建一个对象,命名为 Cell，设置父对象为 Grid，名为i（从0开始）
            var cell = new GameObject("Cell" + i, typeof(RectTransform), typeof(Image));
            cell.transform.SetParent(grid.transform);
            //设置cell的tag为cell
            cell.tag = "Cell";
            //设置cell的锚点x都是0，y都是1。轴心x是0y是1
            var rect = cell.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            //设置cell的大小为规格里的格子边长
            rect.sizeDelta = new Vector2(spec.cellWidth, spec.cellWidth);
            //设置位置，x=totalCells%rows=余，y=totalCells/rows=商，乘以格子边长
            rect.anchoredPosition = new Vector2((i % spec.rows) * spec.cellWidth, -(i / spec.rows) * spec.cellWidth);
            //设置cell的图片精灵为规格里的cell精灵
            cell.GetComponent<Image>().sprite = spec.cellSprite;
            //加入cells列表
            containermod.cells[i] = rect;
        }
        ContainerManager.containers.Add(containermod);
    }
}
}