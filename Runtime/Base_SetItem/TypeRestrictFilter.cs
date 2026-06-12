using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 类型限制过滤器 — 只允许指定 Type 的物品进入。
    /// 装备槽、消耗品槽等场景可直接用。
    ///
    /// Inspector 示例：
    ///   Allowed Types  [1] [2]        ← 只接受 Type=1 或 2 的物品
    ///   Allow Empty    ☑              ← 允许清空格子
    /// </summary>
    [Serializable]
    public class TypeRestrictFilter : SetItemBase
    {
        public int[] allowedTypes;

        public override bool CanExchange(Item incoming, Item outgoing)
        {
            // 清空格子或移出空手 → 放行
            if (incoming.Id == 0) return true;

            if (allowedTypes == null || allowedTypes.Length == 0)
                return true; // 没配白名单 → 放行

            return Array.IndexOf(allowedTypes, incoming.Type) >= 0;
        }
    }
}
