using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public static class CellTouch_Drag
{
    public static void OnDrag(Core core, PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            core.canvas.transform as RectTransform,
            eventData.position,
            core.canvas.worldCamera,
            out Vector2 localPos);
        MiscInit.parent.anchoredPosition = localPos;
    }
}

}
