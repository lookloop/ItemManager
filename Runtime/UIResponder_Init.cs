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
        // 自动构建 + 注册（遍历模板数组，Prefab / 动态 混用）
        if (autoBuild && templates != null && templates.Length > 0)
        {
            foreach (var t in templates)
            {
                GameObject backpack;
                if (t.prefab != null)
                    backpack = BackpackBuilder.BuildFromPrefab(this, t);
                else
                    backpack = BackpackBuilder.Build(this, t);

                int id = ContainerManager.Register(backpack, t);
            }
        }

        // 数据初始化
        BuildData();
    }

    /// <summary>items = 全量数据集，当前页可见窗口</summary>
    public void BuildData()
    {
        ItemDataManager.BuildData(this);
    }

    /// <summary>写入全量数据集。若 index 在当前可见页则同步 UI。</summary>
    public void SetCell(int index, Item item)
    {
        ItemDataManager.SetCellData(this, index, item);
    }

    public void NextPage()
    {
        ItemDataManager.NextPage(this);
    }

    public void PrevPage()
    {
        ItemDataManager.PrevPage(this);
    }

    public void SyncPage()
    {
        ItemDataManager.SyncPage(this);
    }
}
}