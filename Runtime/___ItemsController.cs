using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
/// <summary>
/// Item 数据管理器 — 操控 items 数组 + 顺手同步视图。
/// 外部赋值 items → 自动带动 Cell 视觉刷新。
///
/// ─── 5 个公开方法 ───
///   构建数据    → 按 totalItems/cellCount 创建 items[] + 同步首页
///   设置Cell    → 写入 items[index]，若在当前可见页则同步 UI
///   同步页面    → 整页刷新
///   下一页      → 翻页 + 刷新
///   上一页      → 翻页 + 刷新
/// </summary>
public static class ItemsController
{
    public static void NewItem(ContainerMod containermod, int itemkey, int id, int type, int tier, int count, int[] data)
    {
        if (itemkey < 0 || itemkey >= containermod.items.Length)
        {
            Debug.LogError($"SetItem: itemkey {itemkey} out of range for container with {containermod.items.Length} cells.");
            return;
        }
        containermod.items[itemkey] = new Item(id, type, tier, count, data);
        
    
        
    }
    public static void RefreshItem(ContainerMod containermod, int itemkey)
    {
        Item item = containermod.items[itemkey];
        if (item == null)
            return;
        if(containermod.cells.Length != containermod.items.Length)
        {
            //先用key获取item，看看是否null，如果null就不刷新了
            if (itemkey < containermod.currentPage * containermod.cells.Length && itemkey >= (containermod.currentPage - 1) * containermod.cells.Length)
            {
                int cellkey = itemkey - (containermod.currentPage - 1) * containermod.cells.Length;

            }
        }
    }
    //设置一个方法，使用container.cells，参数为cellkey，使用这个参数，得到一个实际上的cell对象。访问它有没有子对象，如果没有就新建一个，使用预制体新建。然后这个预制体是个空白的带图像的东
    public static void ItemSelfBuild(ContainerMod containermod, RectTransform item)
        {
            var edge = new GameObject("edge", typeof(RectTransform), typeof(Image));
            edge.transform.SetParent(item.transform, false);
            var edgeRect = edge.GetComponent<RectTransform>();
            edgeRect.sizeDelta = new Vector2(item.sizeDelta.x, item.sizeDelta.y);

            var count = new GameObject("count", typeof(RectTransform), typeof(TextMeshProUGUI));
            count.transform.SetParent(item.transform, false);
            var countRect = count.GetComponent<RectTransform>();
            countRect.anchorMin = new Vector2(0.5f, 0);
            countRect.anchorMax = new Vector2(0.5f, 0);
            countRect.pivot = new Vector2(0.5f, 0);
            countRect.sizeDelta = new Vector2(item.sizeDelta.x, item.sizeDelta.y/4);
            countRect.anchoredPosition = new Vector2(0, 0);
        }
}
}
