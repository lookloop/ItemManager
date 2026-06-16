using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    public void SetItem(Container container, int itemKey,
        int id, int type, int tier, int count, int[] data)
    {
        SetItem(container, itemKey, new Item(id, type, tier, count, data));
    }

    public void SetItem(Container container, int itemKey, Item item)
    {
        container.items[itemKey] = item;

        // Notify the filter that data has changed
        container.itemFilter?.OnItemSet(container, itemKey);

        // Skip view refresh if the item lives on a different page
        if (container.items.Length > container.cells.Length)
        {
            int page = itemKey / container.cells.Length + 1;
            if (page != container.currentPage) return;
        }

        Launch(View(container, itemKey));
    }

    /// <summary>
    /// Loads the <c>ItemTable</c> for <c>items[key]</c> asynchronously and
    /// updates the cell's icon, border, and count label.
    /// </summary>
    public async Task View(Container container, int itemKey)
    {
        var item = container.items[itemKey];

        // Map global itemKey to local cellKey
        int cellKey = itemKey;
        if (container.items.Length > container.cells.Length)
            cellKey = itemKey % container.cells.Length;

        var cell = container.cells[cellKey];

        if (item.Id == 0)
        {
            cell.item.gameObject.SetActive(false);
            cell.edge.gameObject.SetActive(false);
            cell.count.gameObject.SetActive(false);
            return;
        }

        var table = await GetItemTable(item.Id.ToString());
        if (table == null) return;

        cell.count.text = item.Count.ToString();
        cell.item.sprite = table.ItemSprite;
        cell.edge.sprite = table.edgeSprite;

        cell.item.gameObject.SetActive(true);
        cell.edge.gameObject.SetActive(true);
        cell.count.gameObject.SetActive(true);
    }

    public void NoView(Container container, int itemKey)
    {
        var item = container.items[itemKey];

        // Map global itemKey to local cellKey
        int cellKey = itemKey;
        if (container.items.Length > container.cells.Length)
            cellKey = itemKey % container.cells.Length;

        var cell = container.cells[cellKey];

        cell.item.gameObject.SetActive(false);
        cell.edge.gameObject.SetActive(false);
        cell.count.gameObject.SetActive(false);
        return;
    }
}
}
