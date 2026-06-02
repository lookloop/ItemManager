using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class UIResponder
{
    void Awake()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        // 自动构建 + 注册（遍历模板数组，Prefab / 动态 混用）
        if (mods != null && mods.Length > 0)
        {
            foreach (var m in mods)
            {
                GameObject backpack;
                if (m.prefab != null)
                    backpack = BackpackBuilder.BuildFromPrefab(this, m);
                else
                    backpack = BackpackBuilder.Build(this, m);

                int id = ContainerManager.Register(backpack, m);
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