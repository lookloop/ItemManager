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

        // Handle the last (partial) page
        if (totalPages > 1 && page == totalPages)
        {
            int lastIndex = container.items.Length - 1;
            if (end > lastIndex) end = lastIndex;
            LastPage(container);
        }
        else
        {
            // Restore full‑page grid height
            int fullRows = Mathf.CeilToInt((float)container.cells.Length / container.row);
            container.gridRect.sizeDelta = new Vector2(
                container.gridRect.sizeDelta.x,
                fullRows * container.cellWidth);

            for (int i = 0; i < container.cells.Length; i++)
                container.cells[i].cell.gameObject.SetActive(true);
        }

        for (int i = start; i <= end; i++)
            Launch(View(container, i));

        // Sync the page‑number input field
        if (container.pageInput != null)
            container.pageInput.text = page + "/" + Mathf.CeilToInt((float)container.items.Length / container.cells.Length);

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

        // After shrinking, clamp y to the valid scroll range
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
        // Force origin.x to 0 — prevents picking up a non‑zero x offset
        // during rapid repeated taps
        Vector2 origin = new Vector2(0f, maskRect.anchoredPosition.y);

        // Slide out
        float elapsed = 0f;
        float outDir = forward ? -1f : 1f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            maskRect.anchoredPosition = new Vector2(origin.x + outDir * maskWidth * t, origin.y);
            yield return null;
        }

        // Slide in
        elapsed = 0f;
        float inStart = forward ? maskWidth : -maskWidth;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            maskRect.anchoredPosition = new Vector2(inStart * (1f - t), origin.y);
            yield return null;
        }

        // Always reset mask x to 0 when done
        maskRect.anchoredPosition = origin;
    }
}
}
