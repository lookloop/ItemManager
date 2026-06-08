using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 物品设置 — 创建 Item 并写入容器，或从 Addressables 加载资源刷新 Cell 显示。
    /// </summary>
    public static class SetItem
    {
        public static void Set(Core core, Container container, int itemKey,
            int id, int type, int tier, int count, int[] data)
        {
            container.items[itemKey] = new Item(id, type, tier, count, data);
            _ = View(core, container, itemKey);
        }

        /// <summary>
        /// 根据 items[key] 的 id 异步加载 ItemTable，刷新 Cell 的图标/边框/数量。
        /// </summary>
        public static async Task View(Core core, Container container, int key)
        {
            var item = container.items[key];
            if (item == null) return;

            var table = await core.GetItemTable(item.Id.ToString());
            if (table == null) return;

            var cell = container.cells[key];
            cell.count.text = item.Count.ToString();
            cell.item.sprite = table.ItemSprite;
            cell.edge.sprite = table.GlowSprite;

            cell.item.gameObject.SetActive(true);
            cell.edge.gameObject.SetActive(true);
            cell.count.gameObject.SetActive(true);
        }
    }
}
