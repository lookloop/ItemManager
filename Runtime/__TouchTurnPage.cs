using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 翻页触控 — 处理 PrevButton / NextButton 的点击翻页逻辑。
    /// </summary>
    public static class TouchTurnPage
    {
        public static void End(Core core, PointerEventData eventData)
        {
            if (core.isDrag)
            return;
            
        }
    }
}
