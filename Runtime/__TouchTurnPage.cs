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

            int totalPages = Mathf.CeilToInt((float)container.items.Length / container.cells.Length);
            page = Mathf.Clamp(page, 1, totalPages);
            SetPage.Set(core, container, page);
        }
    }
}
