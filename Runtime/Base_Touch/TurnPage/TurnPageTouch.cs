using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 翻页按钮交互 — 挂载在 PrevButton / NextButton 上。
    /// </summary>
    public class TurnPageTouch : TouchBase
    {
        /// <summary>-1 = 上一页, +1 = 下一页</summary>
        public int direction;

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            int page = container.currentPage + direction;
            core.SetPage(container, page);
        }
    }
}
