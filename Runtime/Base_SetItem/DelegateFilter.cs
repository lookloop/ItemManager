using System;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Simple delegate filter — does not restrict admission, but fires
    /// a static delegate on every <c>OnItemSet</c> callback.
    /// Subscribe from the main project via <c>DelegateFilter.OnItemChanged += ...</c>.
    /// </summary>
    [Serializable]
    public class DelegateFilter : SetItemBase
    {
        /// <summary>
        /// Fires every time this filter's <c>OnItemSet</c> is called.
        /// </summary>
        public static Action<Container, int> OnItemChanged;

        public override void OnItemSet(Container container, int itemKey)
        {
            base.OnItemSet(container, itemKey);
            OnItemChanged?.Invoke(container, itemKey);
        }
    }
}
