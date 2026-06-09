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

            // 不在当前页则不刷新 UI
            if (container.items.Length > container.cells.Length)
            {
                int page = itemKey / container.cells.Length + 1;
                if (page != container.currentPage) return;
            }

            _ = View(core, container, itemKey);
        }

        /// <summary>
        /// 根据 items[key] 的 id 异步加载 ItemTable，刷新 Cell 的图标/边框/数量。
        /// </summary>
        public static async Task View(Core core, Container container, int itemKey)
        {
            var item = container.items[itemKey];

            // 全局 itemKey → cellKey
            int cellKey = itemKey;
            if (container.items.Length > container.cells.Length)
                cellKey = itemKey % container.cells.Length;

            var cell = container.cells[cellKey];

            if (item == null || item.Id == 0)
            {
                cell.item.gameObject.SetActive(false);
                cell.edge.gameObject.SetActive(false);
                cell.count.gameObject.SetActive(false);
                return;
            }

            var table = await core.GetItemTable(item.Id.ToString());
            if (table == null) return;

            cell.count.text = item.Count.ToString();
            cell.item.sprite = table.ItemSprite;
            cell.edge.sprite = table.GlowSprite;

            cell.item.gameObject.SetActive(true);
            cell.edge.gameObject.SetActive(true);
            cell.count.gameObject.SetActive(true);
        }
    }
}
