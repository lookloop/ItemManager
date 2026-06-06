using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public static class CellTouch_Grid
{
    
    public static void OnDrag(Core core, PointerEventData eventData)
        {
        
        if (CellTouch.grid == null || CellTouch.mask == null) return;

        // 可滚动范围：grid 高度 - mask 高度（<=0 不可滚，>0 可往上滚）
        float maxScroll = CellTouch.grid.sizeDelta.y - CellTouch.mask.sizeDelta.y;
        if (maxScroll <= 0f) return;

        // 起点 → mask 本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CellTouch.mask,
            CellTouch.downEvent.position,
            core.canvas.worldCamera,
            out Vector2 startLocal);

        // 当前 → mask 本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CellTouch.mask,
            eventData.position,
            core.canvas.worldCamera,
            out Vector2 currentLocal);
            

        float deltaY = currentLocal.y - startLocal.y;
        float targetY = CellTouch.gridStartPos.y - deltaY;
        targetY = Mathf.Clamp(targetY, -maxScroll, 0f);
        

        CellTouch.grid.anchoredPosition = new Vector2(0, targetY);
        Debug.Log("1");
        }
}

}
