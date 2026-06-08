using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Cell 触控路由 — 处理物品格子的点击、拖拽、交换等交互。
    /// 由 Core 的 Pointer 事件调用，根据 eventData 命中对象的 tag 分发。
    /// </summary>
    public static class TouchCell
    {
        public static void On(Core core, PointerEventData eventData)
        {

        }

        public static void OnDrag(Core core, PointerEventData eventData)
        {

        }

        public static void End(Core core, PointerEventData eventData)
        {

        }
    }
}
