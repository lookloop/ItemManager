using UnityEngine;

namespace Lookloop.ItemManager
{
    public static class ItemsController
    {

        public static void SetItem(ContainerMod mod, int itemKey, int id, int type, int tier, int count, int[] data)
        {
            if (itemKey < 0 || itemKey >= mod.items.Length)
            {
                Debug.LogError($"[ItemsController] itemKey {itemKey} 越界");
                return;
            }
            mod.items[itemKey] = new Item(id, type, tier, count, data);
            RefreshCell(mod, itemKey);
        }

        /// <summary>移除 key 位置的 Item</summary>
        public static void RemoveItem(ContainerMod mod, int itemKey)
        {
            if (itemKey < 0 || itemKey >= mod.items.Length) return;
            mod.items[itemKey] = null;
            RefreshCell(mod, itemKey);
        }

        /// <summary>交换两个 key 的 Item</summary>
        public static void SwapItem(ContainerMod mod, int keyA, int keyB)
        {
            var tmp = mod.items[keyA];
            mod.items[keyA] = mod.items[keyB];
            mod.items[keyB] = tmp;
            RefreshCell(mod, keyA);
            RefreshCell(mod, keyB);
        }

        public static void NextPage(ContainerMod mod)
        {
            int totalPages = Mathf.CeilToInt((float)mod.items.Length / mod.cells.Length);
            if (mod.currentPage >= totalPages) return;
            mod.currentPage++;
            RefreshPage(mod);
        }

        public static void PrevPage(ContainerMod mod)
        {
            if (mod.currentPage <= 1) return;
            mod.currentPage--;
            RefreshPage(mod);
        }

        public static void RefreshPage(ContainerMod mod)
        {
            int start = (mod.currentPage - 1) * mod.cells.Length;
            for (int i = 0; i < mod.cells.Length; i++)
            {
                int itemKey = start + i;
                Item item = itemKey < mod.items.Length ? mod.items[itemKey] : null;
                ApplyItemUI(mod.itemUIs[i], item);
            }
        }

        static void RefreshCell(ContainerMod mod, int itemKey)
        {
            int cellIndex = itemKey - (mod.currentPage - 1) * mod.cells.Length;
            if (cellIndex >= 0 && cellIndex < mod.cells.Length)
                ApplyItemUI(mod.itemUIs[cellIndex], mod.items[itemKey]);
        }

        /// <summary>把 Item 刷到 ItemUI 上。null → SetActive false。</summary>
        static void ApplyItemUI(ItemUI ui, Item item)
        {
            if (item == null)
            {
                ui.itemImage.gameObject.SetActive(false);
                return;
            }
            ui.itemImage.gameObject.SetActive(true);
            ui.count.text = item.Count > 0 ? item.Count.ToString() : "";
            // TODO: ui.edge.sprite = ...  等 Core.GetItemTable 接入后加载图标
        }
    }
}
