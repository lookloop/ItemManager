using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
    public static class ItemsController
    {
        /// <summary>写入 Item。若在当前页则刷 active/count + 异步加载 Sprite</summary>
        public static async void SetItem(Core core, ContainerMod mod, int itemKey,
            int id, int type, int tier, int count, int[] data)
        {
            if (itemKey < 0 || itemKey >= mod.items.Length)
            {
                Debug.LogError($"[ItemsController] itemKey {itemKey} 越界");
                return;
            }
            mod.items[itemKey] = new Item(id, type, tier, count, data);
            
            int pageIndex = itemKey / mod.cells.Length + 1;
            int cellIndex = itemKey % mod.cells.Length;

            if (pageIndex != mod.currentPage) return;

            var ui = mod.itemUIs[cellIndex];
            ui.itemImage.gameObject.SetActive(true);
            ui.count.text = count > 0 ? count.ToString() : "";

            var table = await core.GetItemTable(id.ToString());
            if (table != null)
            {
                ui.itemImage.sprite = table.ItemSprite;
                ui.edge.sprite = table.GlowSprite;
            }
        }

    }
}
