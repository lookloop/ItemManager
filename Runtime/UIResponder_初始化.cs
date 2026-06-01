using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class UIResponder
{
    void Awake()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        // 缓存 Canvas 摄像机
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCamera = canvas.worldCamera;
    }

    void Start()
    {
        // rows/cols → cellCount/cellsPerRow 自动换算
        cellCount = rows * cols;
        cellsPerRow = cols;

        // 自动构建
        if (autoBuild && gridTransform == null)
            背包初始化.Build(this);

        // 数据初始化
        BuildData();
    }

    /// <summary>items = 全量数据集，_this.items = 当前页可见窗口</summary>
    public void BuildData()
    {
        int dataSize = Mathf.Max(totalItems, cellCount);
        items = new Item[dataSize];
        SyncPage();
    }

    /// <summary>写入全量数据集。若 index 在当前可见页则同步 UI。</summary>
    public void SetCell(int index, Item item)
    {
        if (index < 0 || index >= items.Length) return;
        items[index] = item;

        int pageSize = cellCount;
        int pageStart = currentPage * pageSize;
        if (index >= pageStart && index < pageStart + pageSize)
            背包初始化.设置格子(this, index - pageStart, item);
    }

    public void NextPage()
    {
        int maxPage = Mathf.Max(0, (items.Length - 1) / cellCount);
        if (currentPage < maxPage) currentPage++;
        SyncPage();
    }

    public void PrevPage()
    {
        if (currentPage > 0) currentPage--;
        SyncPage();
    }

    public void SyncPage()
    {
        int pageSize = cellCount;
        int pageStart = currentPage * pageSize;

        for (int i = 0; i < cellCount; i++)
        {
            int dataIndex = pageStart + i;
            if (dataIndex < items.Length)
                背包初始化.设置格子(this, i, items[dataIndex]);
            else
                背包初始化.设置格子(this, i, null);
        }
    }
}
}
