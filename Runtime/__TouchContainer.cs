using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 触控路由 — 静态方法，由 Core 的 Pointer 事件调用。
    /// 根据 eventData 命中对象的 tag 分发到对应处理逻辑。
    /// </summary>
    public static class TouchContainer
    {
       public static void On(Core core)
        {
            // 旧坐标，containertag的特定坐标系的转化结果
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.sourceRect.parent as RectTransform,
                core.eventData.position,
                core.canvas.worldCamera,
                out Vector2 oldLocal);
            core.onPos = oldLocal;

        }

        public static void OnDrag(Core core)
        {
            // 新坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.sourceRect.parent as RectTransform,
                core.eventData.position,
                core.canvas.worldCamera,
                out Vector2 OnDragPos);
            //距离等于新坐标减去旧坐标。
            Vector2 diff = OnDragPos - core.onPos;
            //新的位置设置，原来的位置+距离。将差异弥补，差异是手指移动造成的，也就是有多少差异就移动多少，跟随手指移动。
            core.sourceRect.anchoredPosition = core.sourcePos + diff;
        }

    }
}
