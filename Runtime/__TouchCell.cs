using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Cell 触控路由 — 处理物品格子的点击、拖拽、交换等交互。
    /// 由 Core 的 Pointer 事件调用，根据 eventData 命中对象的 tag 分发。
    /// </summary>
    public static class TouchCell
    {

        public static void On(Core core)
        {
            core.longPressCoroutine = core.StartCoroutine(core.LongPressTimer());
            //初始化grid拖拽
            TouchCell_Grid.On(core);
        }

        public static void OnDrag(Core core)
        {
            if (core.isLongPress)
            {
                TouchItem.OnDrag(core);
            }
            else
            {
                if (core.longPressCoroutine != null)
                {
                    core.StopCoroutine(core.longPressCoroutine);
                    core.longPressCoroutine = null;
                }
                TouchCell_Grid.OnDrag(core);
            }
        }

        public static void End(Core core)
        {

        }
    }
}
