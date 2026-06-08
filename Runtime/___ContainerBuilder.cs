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
    //初始化构建一切资产itemui可用的ui。
    public static void BuildAll(Core core)
    {
        //初始化container数组，长度和检查器定义的数组长度一样。
        core.containers = new Container[core.specs.Length];
        //从零开始挨个将spec转化为container
        for (int i = 0; i < core.specs.Length; i++)
        {
            //新建一个container
            var container = new Container();
            //当前的key。
            core.containers[i] = container;
            //当前的key也是spec的key。
            //如果预制体不是空的，那么按照预制体建造法建造。
            if (core.specs[i].prefabRect != null)
                BuildPrefab(core, core.specs[i], container);
            else
            //如果空了，那么直接按照spec数值建造法。
                Build(core, core.specs[i], container);
        }
    }

    static void BuildPrefab(Core core, ContainerSpec spec, Container container)
    {
        //1把预制体实例化，装载在core下面
        //2将所有子对象引用加入allRects里面
        //3建立一个空list，用于装过滤后的allRects
        var prefabContainer = Object.Instantiate(spec.prefabRect, core.transform);
        var allRects = prefabContainer.GetComponentsInChildren<RectTransform>(true);
        var cellRects = new System.Collections.Generic.List<RectTransform>();
        //这个就是过滤器
        foreach (var allRect in allRects)
        {
            if (allRect.CompareTag("Cell"))
                cellRects.Add(allRect);
        }
        //将所有Cell按照顺序改名，由于是遍历生成的key，所有或许制作预制体，手动排序自己的cell或许可以管理好key，可以看着生成后的cell名字是否符合偏好。
        for (int i = 0; i < cellRects.Count; i++)
        //一行代码不用加括号，这里是命名以后要对应数组里面的key，使用可变长度list，待会变成数组
            cellRects[i].name = i.ToString();
        //这里直接根据list长度变成item实际数据数组，等同长度，意味着页面永远为1。
        container.items = new Item[cellRects.Count];
        //使用预制体的容器Rect是容器Rect
        container.containerRect = prefabContainer;
        //由于是预制体，所以要先创建。
        if(spec.detailRect != null)
            {
                var detailRect = Object.Instantiate(spec.detailRect, prefabContainer.transform);
                container.detailRect  = detailRect;
                container.detailFiller = detailRect.GetComponent<IDetailFiller>();
            }   
        //这里对cell进行改装，用于后续的子对象显示。
        container.cells = BuildCellView(core, container, cellRects);
    }
    //纯粹的数据驱动建造。
    static void Build(Core core, ContainerSpec spec, Container container)
    {
        //使用createRect方法新建一个对象，用于当作container的Rect，设置好tag，方便后续路由。
        var containerRect = CreateRect("Container", core.transform,
            new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f),
            Vector2.zero,
            new(spec.row * spec.cellWidth + spec.containerFillHorizontal * 2,
                spec.maskHeight + spec.containerFillUp + spec.containerFillDown),
            "Container",
            typeof(Image));

        //使用createRect方法新建一个对象，用于mask和grid滑块双人组，用于滚页，是数据驱动独有的，预制体构建没有。
        var maskRect = CreateRect("Mask", containerRect,
            new(0.5f, 1f), new(0.5f, 1f), new(0.5f, 1f),
            new(0, -spec.containerFillUp),
            new(spec.row * spec.cellWidth, spec.maskHeight),
            null,
            typeof(Image), typeof(RectMask2D));

        //使用createRect方法新建一个对象，用于当作grid，用于携带cell数组，是数据驱动独有的，预制体构建没有。
        var gridRect = CreateRect("Grid", maskRect,
            new(0.5f, 1f), new(0.5f, 1f), new(0.5f, 1f),
            Vector2.zero,
            new(spec.row * spec.cellWidth,
                Mathf.CeilToInt((float)spec.everyPageCells / spec.row) * spec.cellWidth),
            "Grid");

        //如果总物品数量大于每页最大格子数量的话，就需要分页了，所以需要分页工具。比如说翻页，和跳页面。
        if (spec.totalItems > spec.everyPageCells)
        {
            // 使用creaRect构建，在最底下，宽高可以自己设置，高度不建议设置为container向下留白还要大，因为那会覆盖到mask上。
            //宽度只要不超过背包宽度，那就应该不突兀。
            var pageTextRect = CreateRect("PageText", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                Vector2.zero,
                new(spec.pageTextWidth, spec.pageTextHeight),
                null,
                typeof(TMP_InputField));

            //area总所周知，tmp的一个光标定位工具，就这样创建，居中它的父级。
            var textArea = CreateRect("Text Area", pageTextRect,
                Vector2.zero, Vector2.one, new(0.5f, 0.5f),
                Vector2.zero, Vector2.zero,
                null,
                typeof(RectMask2D));

            // text，不是input了，而是实实在在的显示层。和上面三个组成三兄弟，输入框，用于跳页。
            var textRect = CreateRect("Text", textArea,
                Vector2.zero, Vector2.one, new(0.5f, 0.5f),
                Vector2.zero, Vector2.zero,
                null,
                typeof(TextMeshProUGUI));
            
            //这里获取tmp这个对象。
            var tmp = pageTextRect.GetComponent<TMP_InputField>();
            //子级是第二兄弟。
            tmp.textViewport = textArea;
            //这里获取第三兄弟，构成tmp输入框。
            tmp.textComponent = textRect.GetComponent<TextMeshProUGUI>();
            //下面是第三兄弟的设置
            tmp.textComponent.font = core.font;
            tmp.textComponent.fontSize = spec.pageTextHeight;
            tmp.textComponent.alignment = TextAlignmentOptions.Center;
            tmp.textComponent.color = Color.white;
            tmp.text = container.currentPage + "/" + Mathf.CeilToInt((float)spec.totalItems / spec.everyPageCells);
            // 强制初始化，tmp的bug，需要重启适应awake的初始化，让Caret正常显示。
            tmp.enabled = false;
            tmp.enabled = true;

            tmp.onSelect.AddListener(delegate { OnPageInputSelect(tmp, container); });
            tmp.onEndEdit.AddListener(delegate(string val) { OnPageInputEndEdit(tmp, container, spec, val); });
            

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

        container.containerRect = containerRect;
        container.maskRect      = maskRect;
        container.gridRect      = gridRect;
        container.items = new Item[spec.totalItems];
        if (spec.detailRect != null)
        {
            container.detailRect = Object.Instantiate(spec.detailRect, containerRect);
            container.detailFiller = container.detailRect.GetComponent<IDetailFiller>();
        }

        var cellRects = new System.Collections.Generic.List<RectTransform>();
        for (int i = 0; i < spec.everyPageCells; i++)
        {
            var rect = CreateRect(i.ToString(), gridRect,
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

    // ─── 内部快捷方法 ───

    /// <summary>遍历 cellRects，为每个 Cell 创建 itemImage + edge + count 子元素，返回 Cell[]</summary>
    static Cell[] BuildCellView(Core core, Container container, System.Collections.Generic.List<RectTransform> cellRects)
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

    // ─── 页码输入 ───

    static void OnPageInputSelect(TMP_InputField tmp, Container container)
    {
        tmp.text = container.currentPage.ToString();
    }

    static void OnPageInputEndEdit(TMP_InputField tmp, Container container, ContainerSpec spec, string val)
    {
        if (int.TryParse(val, out int page))
        {
            int totalPages = Mathf.CeilToInt((float)spec.totalItems / spec.everyPageCells);
            page = Mathf.Clamp(page, 1, totalPages);
            container.currentPage = page;
        }

        int total = Mathf.CeilToInt((float)spec.totalItems / spec.everyPageCells);
        tmp.text = container.currentPage + "/" + total;

        // TODO: 刷新格子
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
