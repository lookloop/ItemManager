using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 测试专用数据填充器 — 仅开发期使用，非生产代码。
    /// </summary>
    public static class TestItemFiller
    {
        /// <summary>
        /// 填充 + 异步加载图片。
        /// 先写入随机 Item 数据，再遍历当前可见页，通过 Addressables 加载 ItemTable，
        /// 把 Sprite 设到对应 ItemUI.edge 上。
        /// </summary>
        public static async void FillAndLoad(Core core, int countPerContainer)
        {
            foreach (var mod in ContainerManager.containers)
            {
                // 1. 写入随机数据（SetItem 自动刷新当前页的 Active + count）
                Fill(mod, countPerContainer);

                // 2. 当前页可见的 item，异步加载图片
                int start = (mod.currentPage - 1) * mod.cells.Length;
                for (int i = 0; i < mod.cells.Length; i++)
                {
                    int itemKey = start + i;
                    if (itemKey >= mod.items.Length) continue;

                    var item = mod.items[itemKey];
                    if (item == null) continue;

                    var table = await core.GetItemTable(item.Id.ToString());
                    if (table != null)
                    {
                        mod.itemUIs[i].itemImage.sprite = table.ItemSprite;
                        mod.itemUIs[i].edge.sprite = table.GlowSprite;
                    }
                }
            }
        }

        static void Fill(ContainerMod mod, int count)
        {
            for (int i = 0; i < count && i < mod.items.Length; i++)
            {
                ItemsController.SetItem(mod, i,
                    id:    Random.Range(1, 5),
                    type:  Random.Range(0, 5),
                    tier:  Random.Range(0, 3),
                    count: Random.Range(1, 99),
                    data:  null
                );
            }
        }

        public static void ClearAll()
        {
            foreach (var mod in ContainerManager.containers)
                Clear(mod);
        }

        static void Clear(ContainerMod mod)
        {
            for (int i = 0; i < mod.items.Length; i++)
                ItemsController.RemoveItem(mod, i);
        }
    }
}
