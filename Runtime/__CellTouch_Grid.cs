using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public static class CellTouch_Grid
{

    public static void OnDrag(Core core, PointerEventData eventData)
    {
        if (CellTouch.grid == null || CellTouch.mask == null) return;

        float maxScroll = CellTouch.grid.sizeDelta.y - CellTouch.mask.sizeDelta.y;
        if (maxScroll <= 0f) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CellTouch.mask,
            eventData.position,
            core.canvas.worldCamera,
            out Vector2 currentLocal);

        float deltaY = currentLocal.y - CellTouch.startLocal.y;
        float targetY = CellTouch.gridStartPos.y + deltaY;
        targetY = Mathf.Clamp(targetY, 0f, maxScroll);

        CellTouch.grid.anchoredPosition = new Vector2(0, targetY);
    }
}

}
