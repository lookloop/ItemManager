using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Lookloop.ItemManager
{
/// <summary>
/// 翻页触控 — Prev/Next 按钮点击翻页。
/// </summary>
public static class TurnPageTouch
{

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
        }
    }

    static void NextPage(Core core, ContainerMod mod)
    {
        int totalPages = Mathf.CeilToInt((float)mod.items.Length / mod.cells.Length);
        if (mod.currentPage >= totalPages) return;
        mod.currentPage++;
        RefreshPage(core, mod);
        UpdatePageText(mod);
    }

    static void PrevPage(Core core, ContainerMod mod)
    {
        if (mod.currentPage <= 1) return;
        mod.currentPage--;
        RefreshPage(core, mod);
        UpdatePageText(mod);
    }

    static void RefreshPage(Core core, ContainerMod mod)
    {
        int start = (mod.currentPage - 1) * mod.cells.Length;
        for (int i = start; i < start + mod.cells.Length && i < mod.items.Length; i++)
            ItemsController.SetViewItem(core, mod, i);
    }

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
