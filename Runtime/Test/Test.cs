using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 测试 — 提供 Init 阶段的调试数据注入。
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
                    // 1/3 概率写入
                    if (Random.Range(0, 3) != 0) continue;

                    SetItem.Set(core, container, itemKey: i,
                        id: Random.Range(1, 5), type: 1, tier: 2, count: Random.Range(1, 100), data: null);
                }
            }
        }
    }
}
