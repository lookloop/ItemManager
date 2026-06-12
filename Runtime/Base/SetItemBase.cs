using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 容器准入过滤器基类 — ScriptableObject，可直接拖入 Inspector。
    ///
    /// 派生示例（装备槽只接受武器）：
    /// <code>
    /// [CreateAssetMenu(menuName = "ItemManager/WeaponOnlyFilter")]
    /// public class WeaponOnlyFilter : SetItemBase
    /// {
    ///     public override bool CanExchange(Item incoming, Item outgoing)
    ///         => incoming.Id == 0 || incoming.Type == (int)ItemType.Weapon;
    /// }
    /// </code>
    ///
    /// 运行时也可直接 new：
    /// <code>
    /// container.itemFilter = ScriptableObject.CreateInstance&lt;WeaponOnlyFilter&gt;();
    /// </code>
    /// </summary>
    public class SetItemBase : ScriptableObject
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
