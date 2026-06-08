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

            var container = core.containers[0];

            SetItem.Set(core, container, itemKey: 0,
                id: 0, type: 1, tier: 2, count: 99, data: null);
        }
    }
}
