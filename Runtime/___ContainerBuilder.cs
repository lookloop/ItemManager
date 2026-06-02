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
        foreach (var mod in core.mods)
        {
            ContainerMod containermod = new ContainerMod();
            if (mod.prefab != null)
                BuildFromPrefab(core, mod.prefab, containermod);
            else
                Build(core, mod);
        }
    }
    /// <summary>
    /// Prefab 模式构建：克隆预制体 → 扫描根下 Cell 子节点 → 注册到全局。
    /// 设计师在 Prefab 里怎么摆，运行时就是什么样，代码不插手布局。
    /// </summary>
    /// <param name="core">Canvas 下的 Core 入口对象</param>
    /// <param name="prefab">用户在 ContainerSpec 里拖入的预制体</param>
    /// <returns>实例化后的容器 GameObject，交给 ContainerManager.Register</returns>
    public static void BuildFromPrefab(Core core, GameObject prefab, ContainerMod containermod)
    {
        var instance = Object.Instantiate(prefab, core.transform);
        {
            var allChildren = instance.GetComponentsInChildren<Transform>(true);
            var list = new System.Collections.Generic.List<GameObject>();
            foreach (var tr in allChildren)
            {
                if (tr.CompareTag("Cell"))
                    list.Add(tr.gameObject);
            }
            for (int i = 0; i < list.Count; i++)
                list[i].name = i.ToString();
            containermod.cells = list.ToArray();
            containermod.items = new Item[containermod.cells.Length];
            containermod.container = instance.transform as RectTransform;
            ContainerManager.containers.Add(containermod);
        }
    }
    public static GameObject Build(Core core, ContainerSpec bp)
    {
        // rows/cols → cellCount/cellsPerRow 自动换算
        ItemTouch.cellCount = bp.rows * bp.cols;
        ItemTouch.cellsPerRow = bp.cols;

        RectTransform root = core.transform as RectTransform;

        // 1. Container 面板
        GameObject panelGo = new GameObject("Container", typeof(RectTransform), typeof(Image));
        panelGo.transform.SetParent(root, false);
        panelGo.tag = "Container";
        var panel = panelGo.transform as RectTransform;
        Image panelImg = panelGo.GetComponent<Image>();
        panelImg.sprite = bp.containerSprite;
        panelImg.type = Image.Type.Sliced;

        // 2. Mask
        GameObject maskGo = new GameObject("Mask", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
        maskGo.transform.SetParent(panelGo.transform, false);
        ItemTouch.maskTransform = maskGo.transform as RectTransform;
        Image maskImg = maskGo.GetComponent<Image>();
        maskImg.sprite = bp.maskSprite;
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
            crt.sizeDelta = new Vector2(bp.cellWidth, bp.cellWidth);
            cell.GetComponent<Image>().sprite = bp.cellSprite;
            cell.tag = "Cell";
            ItemTouch.cellRegistry[i] = cell;
        }

        // 5. 排列 Cell + 设定 Grid/Mask 尺寸
        ApplyCellPositions(bp);
        ApplyGridSize(bp, panel);

        return panelGo;
    }

    static void ApplyCellPositions(ContainerSpec bp)
    {
        float size = bp.cellWidth;
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

    static void ApplyGridSize(ContainerSpec bp, RectTransform panel)
    {
        float size = bp.cellWidth;
        int totalRows = ItemTouch.cellCount / ItemTouch.cellsPerRow + (ItemTouch.cellCount % ItemTouch.cellsPerRow != 0 ? 1 : 0);
        float gridW = ItemTouch.cellsPerRow * size;
        float gridH = totalRows * size;

        ItemTouch.gridTransform.sizeDelta = new Vector2(gridW, gridH);

        if (ItemTouch.maskTransform != null)
        {
            ItemTouch.maskTransform.sizeDelta = new Vector2(gridW, bp.maskHeight);
            ItemTouch.maskTransform.anchoredPosition = new Vector2(0, bp.maskPosY);
        }

        panel.sizeDelta = new Vector2(
            gridW + bp.horizontalPadding * 2f,
            bp.maskHeight + bp.containerExtraHeight);
    }
}
}