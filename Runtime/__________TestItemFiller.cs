using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>测试专用 — 仅开发期使用，非生产代码。</summary>
    public static class TestItemFiller
    {
        /// <summary>
        /// 遍历每个容器的 items[] 全部索引，一半概率填随机数据，一半概率设为 null。
        /// SetItem / RemoveItem 内部会自动刷新视图。
        /// </summary>
        public static void FillAll(Core core)
        {
            foreach (var mod in ContainerManager.containers)
            {
                for (int i = 0; i < mod.items.Length; i++)
                {
                    if (Random.value > 0.5f)
                    {
                        ItemsController.SetItem(core, mod, i,
                            Random.Range(1, 5), 0, 0, Random.Range(1, 99), null);
                    }
                    else
                    {
                        ItemsController.RemoveItem(core, mod, i);
                    }
                }
            }
        }
    }
}
