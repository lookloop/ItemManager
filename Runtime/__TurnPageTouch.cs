using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

namespace Lookloop.ItemManager
{
/// <summary>
/// 翻页触控 — Prev/Next 按钮点击翻页。<br/>
/// 调用链：Core.OnPointerUp → 命中 tag="TurnPage" → Click() → PrevPage/NextPage。
/// </summary>
public static class TurnPageTouch
{
    /// <summary>
    /// 入口：Core.OnPointerUp 在 PointerDownTag=="TurnPage" 且未拖拽时调用。
    /// 按钮名字决定翻页方向，名字打在 ContainerBuilder 创建时。
    /// </summary>
    public static void Click(Core core, PointerEventData eventData)
    {
        if (core.isDrag == true) return;
        
        var go = eventData.pointerCurrentRaycast.gameObject;
        var mod = GetContainerMod(go.transform);
        switch (go.name)
        {
            case "PrevButton":
                PrevPage(core, mod);
                break;
            case "NextButton":
                NextPage(core, mod);
                break;
        }
    }

    public static void NextPage(Core core, ContainerMod mod)
    {
        int totalPages = Mathf.CeilToInt((float)mod.items.Length / mod.cells.Length);
        if (mod.currentPage >= totalPages) return;
        mod.currentPage++;
        RefreshPage(core, mod);
        UpdatePageText(mod);
        core.StartCoroutine(ShakeMask(mod.mask));
    }

    /// <summary>翻前一页：页数 -1 → 刷新格子 → 更新页码文字</summary>
    public static void PrevPage(Core core, ContainerMod mod)
    {
        if (mod.currentPage <= 1) return;
        mod.currentPage--;
        RefreshPage(core, mod);
        UpdatePageText(mod);
        core.StartCoroutine(ShakeMask(mod.mask));
    }

    /// <summary>
    /// 根据 currentPage 把对应区段的 Item 刷到 cells 上显示。<br/>
    /// 例：每页 40 格，第 2 页 → 遍历 items[40..79]，逐个调用 ItemsController.SetViewItem。
    /// </summary>
    static void RefreshPage(Core core, ContainerMod mod)
    {
        int start = (mod.currentPage - 1) * mod.cells.Length;
        for (int i = start; i < start + mod.cells.Length && i < mod.items.Length; i++)
            ItemsController.SetViewItem(core, mod, i);
    }

    /// <summary>
    /// 找到容器根下的 PageText InputField，更新为 "currentPage / totalPages"。
    /// </summary>
    static void UpdatePageText(ContainerMod mod)
    {
        int totalPages = Mathf.CeilToInt((float)mod.items.Length / mod.cells.Length);
        foreach (var input in mod.container.GetComponentsInChildren<TMP_InputField>())
        {
            if (input.gameObject.name == "PageText")
            {
                input.text = mod.currentPage + "/" + totalPages;
                return;
            }
        }
    }

    /// <summary>
    /// 从被点击的 Transform 向上查找，找到所属 ContainerMod。<br/>
    /// 原理：按钮在 containerRect 下 → IsChildOf 匹配 → 返回该 ContainerMod。
    /// </summary>
    static ContainerMod GetContainerMod(Transform t)
    {
        foreach (var mod in ContainerManager.containers)
        {
            if (t.IsChildOf(mod.container))
                return mod;
        }
        return null;
    }

    /// <summary>翻页后 mask 水平晃动两下，纯视觉反馈</summary>
    static IEnumerator ShakeMask(RectTransform mask)
    {
        if (mask == null) yield break;

        float duration = 0.2f;
        float amplitude = 1.5f;
        float freq = 5f; // 约 1 个来回
        float elapsed = 0f;
        Vector2 origin = mask.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float decay = 1f - elapsed / duration;
            float x = origin.x + Mathf.Sin(elapsed * freq * Mathf.PI * 2f) * amplitude * decay;
            mask.anchoredPosition = new Vector2(x, origin.y);
            yield return null;
        }

        mask.anchoredPosition = origin;
    }
}
}
