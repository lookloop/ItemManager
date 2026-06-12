using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 容器准入过滤器基类 — [SerializeReference] 内联展开，直接在 Inspector 面板里配。
    ///
    /// 派生示例（装备槽只接受武器）：
    /// <code>
    /// [Serializable]
    /// public class WeaponOnlyFilter : SetItemBase
    /// {
    ///     public int[] allowedTypes;
    ///
    ///     public override bool CanExchange(Item incoming, Item outgoing)
    ///         => incoming.Id == 0
    ///         || Array.IndexOf(allowedTypes, incoming.Type) >= 0;
    /// }
    /// </code>
    ///
    /// 运行时直接 new 就行：
    /// <code>
    /// container.itemFilter = new WeaponOnlyFilter { allowedTypes = new[] { 1, 2 } };
    /// </code>
    /// </summary>
    [Serializable]
    public class SetItemBase
    {
        /// <summary>
        /// 交换前准入检查。
        /// </summary>
        /// <param name="incoming">将要放入本容器的物品（Id==0 表示对方空手放入空槽）</param>
        /// <param name="outgoing">将从本容器取出的物品（Id==0 表示本槽为空）</param>
        /// <returns>true 允许交换，false 阻止</returns>
        public virtual bool CanExchange(Item incoming, Item outgoing) => true;

        /// <summary>
        /// SetItem 完成后的回调。
        /// </summary>
        /// <param name="container">发生变更的容器</param>
        /// <param name="itemKey">被修改的全局物品 key</param>
        public virtual void OnItemSet(Container container, int itemKey) { }
    }
}
