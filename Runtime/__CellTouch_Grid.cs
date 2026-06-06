using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public static class CellTouch_Grid
{
    


    public static void OnDrag(Core core, PointerEventData eventData)
        {
        if (CellTouch.grid == null) return;

        // 起点 → canvas 坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            core.canvas.transform as RectTransform,
            CellTouch.downEvent.position,
            core.canvas.worldCamera,
            out Vector2 startLocal);

        // 当前 → canvas 坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            core.canvas.transform as RectTransform,
            eventData.position,
            core.canvas.worldCamera,
            out Vector2 currentLocal);

        float deltaY = currentLocal.y - startLocal.y;
        CellTouch.grid.anchoredPosition = CellTouch.gridStartPos + new Vector2(0, deltaY);
        }
}

}
