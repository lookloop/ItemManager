using System.Collections;
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

        public const float pressTime = 0.3f;

        public static bool isLongPress;

        static Coroutine longPressCoroutine;

        public static void On(Core core, PointerEventData eventData)
        {
            //启动协程
            longPressCoroutine = core.StartCoroutine(LongPressTimer());
            //初始化grid拖拽
            TouchCell_Grid.On(core, eventData);
        }

        public static void OnDrag(Core core, PointerEventData eventData)
        {
            if (isLongPress)
            {
                // 长按已触发，拖拽中
            }
            else
            {
                if (longPressCoroutine != null)
                {
                    core.StopCoroutine(longPressCoroutine);
                    longPressCoroutine = null;
                }
                TouchCell_Grid.OnDrag(core, eventData);
            }
        }

        public static void End(Core core, PointerEventData eventData)
        {

        }

        static IEnumerator LongPressTimer()
        {
            yield return new WaitForSeconds(pressTime);
            isLongPress = true;

            while (true)
            {
                yield return null;
            }
        }
    }
}
