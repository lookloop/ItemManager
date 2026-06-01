using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
/// <summary>
/// <summary>
/// D 长按结算 — isDrag? 交换/装备 : 复位。合并了原 D_LongPressClick + D_LongPressDragEnd
/// </summary>
public static class D_LongPressEnd
{
    public static void Execute(UIResponder _this, PointerEventData eventData)
    {
        if (_this.isDrag)
            结算拖拽(_this, eventData);
        else
            复位(_this);

        _this.ClearDragState();
    }

    // ── 复位：物品回原格子 ──
    private static void 复位(UIResponder _this)
    {
        if (_this.sourceItem != null && _this.sourceObject != null)
        {
            _this.sourceItem.transform.SetParent(_this.sourceObject.transform, false);
            _this.sourceItem.transform.localScale = Vector3.one;
            if (_this.sourceItem.transform is RectTransform rt)
                rt.anchoredPosition = Vector2.zero;
        }
    }

    // ── 拖拽结算：格子交换 > 复位 ──
    private static void 结算拖拽(UIResponder _this, PointerEventData eventData)
    {
        // 1. 格子交换
        if (尝试交换(_this)) return;

        // 2. 不行 → 复位
        复位(_this);
    }

    private static bool 尝试交换(UIResponder _this)
    {
        if (_this.targetObject == null || _this.targetObject == _this.sourceObject)
            return false;

        var cont = _this.GetContainerData(_this.sourceObject.transform);
        if (cont == null || cont.items == null) return false;

        int srcIdx = System.Array.IndexOf(_this.cellRegistry, _this.sourceObject);
        int dstIdx = System.Array.IndexOf(_this.cellRegistry, _this.targetObject);
        if (srcIdx < 0 || dstIdx < 0 || srcIdx >= cont.items.Length || dstIdx >= cont.items.Length)
            return false;

        // 交换数据
        var srcItem = cont.items[srcIdx];
        var dstItem = cont.items[dstIdx];
        背包初始化.设置格子(_this, srcIdx, dstItem);
        背包初始化.设置格子(_this, dstIdx, srcItem);

        // 销毁漂浮物
        if (_this.sourceItem != null) Object.Destroy(_this.sourceItem);

        Debug.Log($"交换: {srcIdx} ↔ {dstIdx}");
        return true;
    }

}
}
