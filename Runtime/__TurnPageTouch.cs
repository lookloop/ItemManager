using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
/// <summary>
/// 翻页触控 — Prev/Next 按钮点击 + PageText 点击跳页。
/// </summary>
public static class TurnPageTouch
{
    static TMP_InputField _input;

    public static void Click(Core core, PointerEventData eventData)
    {
        var go = eventData.pointerCurrentRaycast.gameObject;
        if (go == null) return;

        var mod = GetContainerMod(go.transform);
        if (mod == null) return;

        switch (go.name)
        {
            case "PrevButton":
                PrevPage(core, mod);
                break;
            case "NextButton":
                NextPage(core, mod);
                break;
            case "PageText":
                ShowPageInput(core, mod, go.GetComponent<RectTransform>());
                break;
        }
    }

    static void NextPage(Core core, ContainerMod mod)
    {
        int totalPages = Mathf.CeilToInt((float)mod.items.Length / mod.cells.Length);
        if (mod.currentPage >= totalPages) return;
        mod.currentPage++;
        RefreshPage(core, mod);
    }

    static void PrevPage(Core core, ContainerMod mod)
    {
        if (mod.currentPage <= 1) return;
        mod.currentPage--;
        RefreshPage(core, mod);
    }

    static void RefreshPage(Core core, ContainerMod mod)
    {
        int start = (mod.currentPage - 1) * mod.cells.Length;
        for (int i = start; i < start + mod.cells.Length && i < mod.items.Length; i++)
            ItemsController.SetViewItem(core, mod, i);
    }

    static void ShowPageInput(Core core, ContainerMod mod, RectTransform pageTextRect)
    {
        if (_input != null) return; // 已弹着

        var go = new GameObject("PageInput", typeof(RectTransform), typeof(TMP_InputField));
        go.transform.SetParent(pageTextRect, false);
        go.transform.SetAsLastSibling();

        var rect = go.transform as RectTransform;
        rect.anchorMin = rect.anchorMax = Vector2.one * 0.5f;
        rect.sizeDelta = pageTextRect.sizeDelta;
        rect.anchoredPosition = Vector2.zero;

        _input = go.GetComponent<TMP_InputField>();
        _input.text = mod.currentPage.ToString();
        _input.onSubmit.AddListener(val =>
        {
            if (int.TryParse(val, out int page))
                GoToPage(core, mod, page);
            Object.Destroy(go);
            _input = null;
        });
        _input.Select();
        _input.ActivateInputField();
    }

    static void GoToPage(Core core, ContainerMod mod, int page)
    {
        int totalPages = Mathf.CeilToInt((float)mod.items.Length / mod.cells.Length);
        page = Mathf.Clamp(page, 1, totalPages);
        mod.currentPage = page;
        RefreshPage(core, mod);
    }

    static ContainerMod GetContainerMod(Transform t)
    {
        foreach (var mod in ContainerManager.containers)
        {
            if (t.IsChildOf(mod.container))
                return mod;
        }
        return null;
    }
}
}
