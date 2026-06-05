using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
    public static class ItemsController
    {
        /// <summary>写入 Item。若在当前页则刷 active/count + 异步加载 Sprite</summary>
        public static void SetItem(Core core, ContainerMod mod, int itemKey,
            int id, int type, int tier, int count, int[] data)
        {
            if (itemKey < 0 || itemKey >= mod.items.Length)
            {
                Debug.LogError($"[ItemsController] itemKey {itemKey} 越界");
                return;
            }
            // id==0 → 视为空
            if (id == 0)
                mod.items[itemKey] = null;
            else
                mod.items[itemKey] = new Item(id, type, tier, count, data);

            SetViewItem(core, mod, itemKey);
        }

        public static async void SetViewItem(Core core, ContainerMod mod, int itemKey)
        {
            int pageIndex = itemKey / mod.cells.Length + 1;
            int cellIndex = itemKey % mod.cells.Length;

            if (pageIndex != mod.currentPage) return;

            var ui = mod.itemUIs[cellIndex];
            var item = mod.items[itemKey];

            // null → 三件套全隐藏
            if (item == null)
            {
                ui.itemImage.gameObject.SetActive(false);
                ui.edge.gameObject.SetActive(false);
                ui.count.gameObject.SetActive(false);
                return;
            }

            ui.itemImage.gameObject.SetActive(true);
            ui.edge.gameObject.SetActive(true);
            ui.count.gameObject.SetActive(true);
            ui.count.text = item.Count > 0 ? item.Count.ToString() : "";

            var table = await core.GetItemTable(item.Id.ToString());
            if (table != null)
            {
                ui.itemImage.sprite = table.ItemSprite;
                ui.edge.sprite = table.GlowSprite;
            }
        }


    }
}
