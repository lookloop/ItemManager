using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Animations;

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
            var parent = (RectTransform)core.sourceRect.parent;
            // 旧坐标 — 按下时的屏幕坐标转为当前父级本地坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 oldLocal);
                core.onPos = oldLocal;
        }

        public static void OnDrag(Core core, PointerEventData eventData)
        {
            var parent = (RectTransform)core.sourceRect.parent;
            // 新坐标 — 实时屏幕坐标转为当前父级本地坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent,
                eventData.position,
                core.canvas.worldCamera,
                out Vector2 newLocal);

            Vector2 delta = newLocal - core.onPos;
            core.sourceRect.anchoredPosition = core.sourcePos + delta;
        }
        public static void End(Core core, PointerEventData eventData)
        {
            
        }

    }
}
