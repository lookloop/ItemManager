using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 测试用过滤器 — 只允许奇数 Id 的物品进入。
    /// 验证 [SerializeReference] 类型下拉 + CanExchange 生效。
    /// </summary>
    [Serializable]
    public class OddIdOnlyFilter : SetItemBase
    {
        public bool verbose;

        public override bool CanExchange(Item incoming, Item outgoing)
        {
            bool ok = incoming.Id == 0 || incoming.Id % 2 == 1;
            if (verbose && !ok)
                UnityEngine.Debug.Log($"[OddIdOnlyFilter] 拒绝 Id={incoming.Id}");
            return ok;
        }

        public override void OnItemSet(Container container, int itemKey)
        {
            if (verbose)
            {
                var item = container.items[itemKey];
                UnityEngine.Debug.Log(
                    $"[OddIdOnlyFilter] container[{itemKey}] ← Id={item.Id}");
            }
        }
    }
}
