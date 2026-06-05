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
            mod.items[itemKey] = new Item(id, type, tier, count, data);
            //使用下面那个方法
            SetViewItem(core,mod,itemKey);
            
            
        }
        //设置一个itemview方法，像上面那个一样
        public static async void SetViewItem(Core core, ContainerMod mod, int itemKey)
        {
            int pageIndex = itemKey / mod.cells.Length + 1;
            int cellIndex = itemKey % mod.cells.Length;

            if (pageIndex != mod.currentPage) return;

            var item = mod.items[itemKey];
            if (item == null) return;

            int id = item.Id;
            int count = item.Count;
            var ui = mod.itemUIs[cellIndex];

            // id == 0 视为空 — 三件套全隐藏
            if (id == 0)
            {
                ui.itemImage.gameObject.SetActive(false);
                ui.edge.gameObject.SetActive(false);
                ui.count.gameObject.SetActive(false);
                return;
            }

            ui.itemImage.gameObject.SetActive(true);
            ui.edge.gameObject.SetActive(true);
            ui.count.gameObject.SetActive(true);
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
