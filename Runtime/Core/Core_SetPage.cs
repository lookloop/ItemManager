using System.Collections;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    public void SetPage(Container container, int page)
    {
        int oldPage = container.currentPage;
        int totalPages = Mathf.CeilToInt((float)container.items.Length / container.cells.Length);
        page = Mathf.Clamp(page, 1, totalPages);
        container.currentPage = page;

        int start = container.cells.Length * (page - 1);
        int end = container.cells.Length * page - 1;

        // 检测最后一页
        if (totalPages > 1 && page == totalPages)
        {
            int lastIndex = container.items.Length - 1;
            if (end > lastIndex) end = lastIndex;
            LastPage(container);
        }
        else
        {
            // 恢复满页 grid 高度
            int fullRows = Mathf.CeilToInt((float)container.cells.Length / container.row);
            container.gridRect.sizeDelta = new Vector2(
                container.gridRect.sizeDelta.x,
                fullRows * container.cellWidth);

            for (int i = 0; i < container.cells.Length; i++)
                container.cells[i].cell.gameObject.SetActive(true);
        }

        for (int i = start; i <= end; i++)
            FireAndForget(View(container, i));

        // 同步 TMP 翻页输入框显示
        if (container.pageInput != null)
            container.pageInput.text = page + "/" + Mathf.CeilToInt((float)container.items.Length / container.cells.Length);

        // 翻页后检测：当前 container 是否是 sourceContainer，且 sourceItemKey 是否在当前页
        if (container == dragSourceContainer &&
            dragSourceItemKey >= start && dragSourceItemKey <= end)
        {
            NoView(container, dragSourceItemKey);
        }

        if (page != oldPage)
            StartCoroutine(FlipFeedback(container, page > oldPage));
    }

    static void LastPage(Container container)
    {
        int lastItemCount = container.items.Length % container.cells.Length;
        if (lastItemCount == 0) lastItemCount = container.cells.Length;

        int rows = Mathf.CeilToInt((float)lastItemCount / container.row);
        container.gridRect.sizeDelta = new Vector2(
            container.gridRect.sizeDelta.x,
            rows * container.cellWidth);

        // 高度缩小后，钳制 y 到合法范围
        float gridH = container.gridRect.sizeDelta.y;
        float maskH = container.maskRect.sizeDelta.y;
        float maxY = Mathf.Max(0f, gridH - maskH);
        float y = Mathf.Clamp(container.gridRect.anchoredPosition.y, 0f, maxY);
        container.gridRect.anchoredPosition = new Vector2(container.gridRect.anchoredPosition.x, y);

        for (int i = lastItemCount; i < container.cells.Length; i++)
            container.cells[i].cell.gameObject.SetActive(false);
    }

    IEnumerator FlipFeedback(Container container, bool forward)
    {
        var maskRect = container.maskRect;
        if (maskRect == null) yield break;

        float maskWidth = maskRect.rect.width;
        float half = flipDuration / 2f;
        // origin.x 强制为 0，防止快速连点时捕获到非零的偏移 x
        Vector2 origin = new Vector2(0f, maskRect.anchoredPosition.y);

        // 滑出
        float elapsed = 0f;
        float outDir = forward ? -1f : 1f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            maskRect.anchoredPosition = new Vector2(origin.x + outDir * maskWidth * t, origin.y);
            yield return null;
        }

        // 滑入
        elapsed = 0f;
        float inStart = forward ? maskWidth : -maskWidth;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            maskRect.anchoredPosition = new Vector2(inStart * (1f - t), origin.y);
            yield return null;
        }

        // 结束必须 mask x 清零
        maskRect.anchoredPosition = origin;
    }
}
}
