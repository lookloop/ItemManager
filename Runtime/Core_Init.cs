using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        ContainerBuilder.BuildAll(this);
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