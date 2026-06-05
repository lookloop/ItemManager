using UnityEngine;

namespace Lookloop.ItemManager
{
    public static class ItemsController
    {

        public static void SetItem(Core core, ContainerMod mod, int itemKey, int id, int type, int tier, int count, int[] data)
        {
            if (itemKey < 0 || itemKey >= mod.items.Length)
            {
                Debug.LogError($"[ItemsController] itemKey {itemKey} 越界");
                return;
            }
            mod.items[itemKey] = new Item(id, type, tier, count, data);

        }

    }
}
