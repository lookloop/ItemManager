using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 翻页按钮交互 — 挂载在 PrevButton / NextButton 上。
    /// </summary>
    public class TurnPageHandler : ItemHandler
    {
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != 0) return;

            int page = container.currentPage;

            switch (gameObject.name)
            {
                case "PrevButton": page--; break;
                case "NextButton": page++; break;
                default: return;
            }

            SetPage.Set(core, container, page);
        }
    }
}
