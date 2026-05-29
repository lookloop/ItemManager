using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
public static class D_短按拖拽结束
{
    public static void Execute(UIResponder _this, PointerEventData eventData)
    {
        // 短按拖拽 = 滚动 Grid 或拖动背包面板。松手时钳位边界，防止滑出可视范围。
        if (_this.sourceObject != null && _this.sourceObject.CompareTag("背包"))
        {
            // 拖动背包面板 — 面板位置已在 短按拖拽中 实时更新，松手无需额外处理
            return;
        }

        // 滚动 Grid — 钳位到合法滚动范围
        if (_this.gridTransform != null && _this.maskTransform != null)
        {
            float maskHeight = _this.maskTransform.rect.height;
            float gridHeight = _this.gridTransform.rect.height;
            float maxScroll = Mathf.Max(0, gridHeight - maskHeight);

            Vector2 pos = _this.gridTransform.anchoredPosition;
            pos.y = Mathf.Clamp(pos.y, 0, maxScroll);
            _this.gridTransform.anchoredPosition = pos;
        }
    }
}
}
