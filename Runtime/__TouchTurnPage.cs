using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 翻页触控 — 处理 PrevButton / NextButton 的点击翻页逻辑。
    /// </summary>
    public static class TouchTurnPage
    {
        public static void End(Core core)
        {
            if (core.isDrag || core.sourceContainer == null) return;

            var container = core.sourceContainer;
            int page = container.currentPage;

            switch (core.sourceRect.name)
            {
                case "PrevButton":
                    page--;
                    break;
                case "NextButton":
                    page++;
                    break;
                default:
                    return;
            }

            SetPage.Set(core, container, page);
        }
    }
}
