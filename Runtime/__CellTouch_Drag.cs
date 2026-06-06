using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public static class CellTouch_Drag
{
    public static void OnDrag(Core core, PointerEventData eventData)
    {
        // 跟手
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            core.canvas.transform as RectTransform,
            eventData.position,
            core.canvas.worldCamera,
            out Vector2 localPos);
        MiscInit.parent.anchoredPosition = localPos;

        // 检测手指下是否命中 Cell
        var hitGo = eventData.pointerCurrentRaycast.gameObject;
        if (hitGo == null || !hitGo.CompareTag("Cell")) return;

        var hitCell = hitGo.transform as RectTransform;
        if (hitCell == null) return;

        // 判断该 Cell 是否有 Grid 父级
        var grid = hitCell.parent as RectTransform;
        if (grid == null)
        {
            // 无 Grid 路线
        }
        else
        {
            // 有 Grid 路线 → 获取 Mask
            var mask = grid.parent as RectTransform;
        }
    }
}

}
