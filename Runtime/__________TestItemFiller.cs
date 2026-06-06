using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>测试专用 — 仅开发期使用，非生产代码。</summary>
    public static class TestItemFiller
    {
        /// <summary>给所有容器塞随机 Item。SetItem 内部自动加载 Sprite。</summary>
        public static void FillAll(Core core, int countPerContainer)
        {
            foreach (var mod in ContainerManager.containers)
            {
                for (int i = 0; i < countPerContainer && i < mod.items.Length; i++)
                {
                    ItemsController.SetItem(core, mod, i,
                        Random.Range(1, 5), 0, 0, Random.Range(1, 99), null);
                }
            }
        }
    }
}
