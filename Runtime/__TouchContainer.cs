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
