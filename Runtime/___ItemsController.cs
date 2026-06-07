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
            SetViewItem(core, mod, itemKey);
        }

        /// <summary>移除指定 key 的 Item</summary>
        public static void RemoveItem(Core core, ContainerMod mod, int itemKey)
        {
            if (itemKey < 0 || itemKey >= mod.items.Length) return;
            mod.items[itemKey] = null;
            SetViewItem(core, mod, itemKey);
        }

        /// <summary>交换两个 key 的 Item。null 照换不误。</summary>
        public static void SwapItem(Core core, ContainerMod mod, int keyA, int keyB)
        {
            var keya = mod.items[keyA];
            mod.items[keyA] = mod.items[keyB];
            mod.items[keyB] = keya;
            SetViewItem(core, mod, keyA);
            SetViewItem(core, mod, keyB);
        }

        public static async void SetViewItem(Core core, ContainerMod mod, int itemKey)
        {
            int pageIndex = itemKey / mod.cells.Length + 1;
            int cellIndex = itemKey % mod.cells.Length;

            if (pageIndex != mod.currentPage) return;

            var ui = mod.itemUIs[cellIndex];
            var item = mod.items[itemKey];

            // null 或 Id=0 → 三件套全隐藏
            if (item == null || item.Id == 0)
            {
                if (item != null)
                    Debug.LogWarning($"[SetViewItem] Id=0 无效 — itemKey:{itemKey}");
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

        /// <summary>强制隐藏指定 itemKey 的 ItemUI（若在当前页）。拖拽拾取时用。</summary>
        public static void HideItemUI(ContainerMod mod, int itemKey)
        {
            int pageIndex = itemKey / mod.cells.Length + 1;
            if (pageIndex != mod.currentPage) return;
            int cellIndex = itemKey % mod.cells.Length;
            var ui = mod.itemUIs[cellIndex];
            ui.itemImage.gameObject.SetActive(false);
            ui.edge.gameObject.SetActive(false);
            ui.count.gameObject.SetActive(false);
        }
    }
}
