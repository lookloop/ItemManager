using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Page‑flip button handler. Attached to the PrevButton / NextButton
    /// RectTransforms at build time.
    /// </summary>
    public class TurnPageTouch : TouchBase
    {
        /// <summary>-1 = previous page, +1 = next page</summary>
        public int direction;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;
            FocusContainer();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            int page = container.currentPage + direction;
            core.SetPage(container, page);
        }
    }
}
