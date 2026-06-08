using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 触控路由 — 静态方法，由 Core 的 Pointer 事件调用。
    /// 根据 eventData 命中对象的 tag 分发到对应处理逻辑。
    /// </summary>
    public static class TouchContainer
    {
        /// <summary>
        /// 手指按下
        /// </summary>
        public static void On(Core core, PointerEventData eventData)
        {
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (target == null) return;
        }

    }
}
