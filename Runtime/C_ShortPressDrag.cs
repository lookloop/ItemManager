using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
/// <summary>C 短按拖拽中 — DragPanel(拖面板) / ScrollGrid(滚Grid，X锁定+Y钳位)</summary>
public static class C_ShortPressDrag
{
    /// <summary>拖拽容器面板 — dragTarget/dragStartPos 在 A_PointerDown 已存</summary>
    public static void DragPanel(UIResponder _this, PointerEventData eventData)
    {
        _this.endPosition = UI坐标转换.获取事件局部坐标(_this.canvas.transform as RectTransform, eventData);
        Vector2 totalDelta = _this.endPosition - _this.beginPosition;
        if (_this.dragTarget != null)
            _this.dragTarget.anchoredPosition = _this.dragStartPos + totalDelta;
    }

    /// <summary>滚动 Grid — dragTarget/dragStartPos 在 A_PointerDown 已存</summary>
    public static void ScrollGrid(UIResponder _this, PointerEventData eventData)
    {
        RectTransform gridRT = _this.dragTarget;
        if (gridRT == null) return;
        RectTransform maskRT = gridRT.parent as RectTransform;
        if (maskRT == null) return;

        _this.endPosition = UI坐标转换.获取事件局部坐标(_this.canvas.transform as RectTransform, eventData);
        Vector2 totalDelta = _this.endPosition - _this.beginPosition;

        // X 轴锁定归零，只响应 Y 轴
        float maskHeight = maskRT.rect.height;
        float gridHeight = gridRT.rect.height;
        float diff = Mathf.Max(0, gridHeight - maskHeight);

        float targetY = _this.dragStartPos.y + totalDelta.y;
        targetY = Mathf.Clamp(targetY, 0, diff);

        gridRT.anchoredPosition = new Vector2(0, targetY);
    }
}
}
