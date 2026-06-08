using System.Collections.Generic;
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
        var cellRects = new List<RectTransform>();
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
        //创建一个详情面板，基于详情面板预制体，所以没有创建细节，无论container是数据创建还是预制体创建，detail都是预制体创建，并且在自己的脚本实现接口。
        if(spec.detailRect != null)
            {
                container.detailRect = Object.Instantiate(spec.detailRect, prefabContainer.transform);
                container.detailFiller = container.detailRect.GetComponent<IDetailFiller>();
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

            ///tmp的输入框，点击后聚焦，聚焦的意思就是键盘输入可以录入输入框。
            ///tmp可以定义，聚焦开始和结束要启用什么方法。
            ///这是聚焦开始，点击input就启用。
            tmp.onSelect.AddListener(delegate { OnPageInput(tmp, container); });
            //聚焦开始后的聚焦结束，enter，或者是点击其他地方触发。
            tmp.onEndEdit.AddListener(delegate { OffPageInput(tmp, container, spec); });
            

            //这里是向左翻页按钮。在主控core通过tag分析，进入路由模型后进行执行操作，这里没有执行。
            //刻意挨着tmp，是一个系列的ui组件。
            var prevButtonRect = CreateRect("PrevButton", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                new(-spec.pageTextWidth / 2 - spec.pageTextHeight / 2, 0),
                new(spec.pageTextHeight, spec.pageTextHeight),
                "TurnPage",
                typeof(Image));

            //这里是向右翻页按钮。在主控core通过tag分析，进入路由模型后进行执行操作，这里没有执行。
            //刻意挨着tmp，是一个系列的ui组件。
            var nextButtonRect = CreateRect("NextButton", containerRect,
                new(0.5f, 0f), new(0.5f, 0f), new(0.5f, 0f),
                new(spec.pageTextWidth / 2 + spec.pageTextHeight / 2, 0),
                new(spec.pageTextHeight, spec.pageTextHeight),
                "TurnPage",
                typeof(Image));
        }
        //设置图像，让图像看起来好看一些，用玩家自定义的图像。
        containerRect.GetComponent<Image>().sprite = spec.containerSprite;
        maskRect.GetComponent<Image>().sprite = spec.maskSprite;
        //留下引用，方便container后续寻找
        container.containerRect = containerRect;
        //翻页双人组，mask和grid，是预制体创建没有的，预制体创建为null。
        container.maskRect      = maskRect;
        container.gridRect      = gridRect;
        //根据用于定义的广度进行设置这个container存储item的总量。
        container.items = new Item[spec.totalItems];
        //创建一个详情面板，基于详情面板预制体，所以没有创建细节，无论container是数据创建还是预制体创建，detail都是预制体创建，并且在自己的脚本实现接口。
        if (spec.detailRect != null)
        {
            container.detailRect = Object.Instantiate(spec.detailRect, containerRect);
            container.detailFiller = container.detailRect.GetComponent<IDetailFiller>();
        }
        //和上面一样，整一个list装cell。至于为什么不是直接数组反正有everyPageCells，
        //因为上面预制体创建用的就是list作为参数传递进入BuildCellView，这里也同步一下，复用一下方法。
        var cellRects = new List<RectTransform>();
        //那个是天然就有预制体，这个是自己循环创建，并且依次排好位置，符合容器横平竖直排列规范。
        for (int i = 0; i < spec.everyPageCells; i++)
        {
            //每一个循环都要新建一个，填写很多参数进去。
            var rect = CreateRect(i.ToString(), gridRect,
                new(0f, 1f), new(0f, 1f), new(0f, 1f),
                new((i % spec.row) * spec.cellWidth, -(i / spec.row) * spec.cellWidth),
                new(spec.cellWidth, spec.cellWidth),
                "Cell",
                typeof(Image));
                //这里创建完毕了。
            //给这个格子背景上色，使用用户拖进来的精灵图。
            rect.GetComponent<Image>().sprite = spec.cellSprite;
            //加入list。
            cellRects.Add(rect);
        }
        //使用这个方法传递list，生成数组，并且完成cellview的基础设施构建工作。
        container.cells = BuildCellView(core, container, cellRects);
    }

    // ─── 内部快捷方法 ───

    //使用list，遍历list，针对每一个cell进行生成对应的view配置。返回一个数组，作为container的一部分。
    static Cell[] BuildCellView(Core core, Container container, List<RectTransform> cellRects)
    {
        //直接通过list的count来决定数组长度，同时也是everypageCells参数，值是一样的。因为list就是基于everyPageCell创建的。
        var cells = new Cell[cellRects.Count];
        //遍历这些cell。
        for (int i = 0; i < cellRects.Count; i++)
        {
            //i就是key，依次访问。
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

    static void OnPageInput(TMP_InputField tmp, Container container)
    {
        //设置text为当前页，聚焦时直接设置这个数值，让玩家自己在此之上修改。
        tmp.text = container.currentPage.ToString();
    }

    static void OffPageInput(TMP_InputField tmp, Container container, ContainerSpec spec)
    {
        //格子总数/每一页的cell总数，向上取整，mathf.ceilToInt是向上取整专用数学工具。
        int pagemax = Mathf.CeilToInt((float)spec.totalItems / spec.everyPageCells);
        //尝试把用户输入的string转化为int。如果不成功就跳过，不报错，用户必须输入纯数字才行，才有效果。
        if (int.TryParse(tmp.text, out int page))
        {
            
            //如果转化page成功，那么我们知道全部页面max和全部页面最小值，一般来说最小值就是1。
            //Mathf.Clamp，将第一个参数，如果这个参数小于1，那么就等于1，如果大于totalpages，那么就等于totalpages。
            //如果在1和totalpages之间，那么页数就等于原值。用于防止用户输出超额。
            page = Mathf.Clamp(page, 1, pagemax);
            //直接将当前页设置为这个数字，实现数据上的跳转页面。
            container.currentPage = page;
        }

        
        tmp.text = container.currentPage + "/" + pagemax;

        //后续可以增加刷新，格子，现在暂时留空，ai下次读到就提醒我。
    }












    /// 创建 RectTransform，一步到位：父物体、锚点、pivot、位置、尺寸、tag、组件，用于unity的对象创建，让调用方看起来简洁一些。
    static RectTransform CreateRect(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPosition, Vector2 sizeDelta,
        string tag = null,
        params System.Type[] components)
    {
        //这里方法开始，上面一大坨是参数。
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
