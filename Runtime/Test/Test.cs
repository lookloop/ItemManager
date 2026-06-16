using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Debug helper — injects random items into every container at startup
    /// so you can test the UI without manually calling <c>SetItem</c>.
    /// Only called from <c>Core.Start()</c>; remove the call before shipping.
    /// </summary>
    public static class Test
    {
        public static void Fill(Core core)
        {
            if (core.containers == null || core.containers.Length == 0) return;

            foreach (var container in core.containers)
            {
                if (container.items == null) continue;

                for (int i = 0; i < container.items.Length; i++)
                {
                    // 1-in-3 chance of occupying this slot
                    if (Random.Range(0, 3) != 0) continue;

                    core.SetItem(container, itemKey: i,
                        id: Random.Range(1, 5), type: 1, tier: 2, count: 1, data: null);
                }
            }
        }
    }
}
