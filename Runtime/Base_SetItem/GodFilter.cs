using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 上帝过滤器 — 交换准入和回调都通过委托广播到主项目。
    /// 主项目订阅 <c>OnCanExchange</c> 判准入、<c>OnItemChanged</c> 接回调。
    /// </summary>
    [Serializable]
    public class GodFilter : SetItemBase
    {
        /// <summary>
        /// 交换准入委托。主项目返回 true 放行、false 拒绝。
        /// container: 容器, incoming: 进来的道具, outgoing: 出去的（Id=0 为空）。
        /// </summary>
        public static Func<Container, Item, Item, bool> OnCanExchange;

        /// <summary>
        /// 道具变更回调。主项目在此处理换装、存档等后续逻辑。
        /// </summary>
        public static Action<Container, int> OnItemChanged;

        public override bool CanExchange(Item incoming, Item outgoing)
        {
            return OnCanExchange?.Invoke(container, incoming, outgoing) ?? true;
        }

        public override void OnItemSet(Container container, int itemKey)
        {
            base.OnItemSet(container, itemKey);
            OnItemChanged?.Invoke(container, itemKey);
        }
    }
}
