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

    // ── 拖拽结算：装备槽 > 格子交换 > 复位 ──
    private static void 结算拖拽(UIResponder _this, PointerEventData eventData)
    {
        // 1. 装备槽检测
        if (尝试装备(_this, eventData)) return;

        // 2. 格子交换
        if (尝试交换(_this)) return;

        // 3. 都不行 → 复位
        复位(_this);
    }

    private static bool 尝试装备(UIResponder _this, PointerEventData eventData)
    {
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        int slotIndex = GetEquipSlotIndex(_this, dropTarget);
        if (slotIndex < 0 || _this.sourceObject == null) return false;

        var container = _this.GetContainerData(_this.sourceObject.transform);
        if (container == null || container.items == null) return false;

        int sourceIndex = System.Array.IndexOf(_this.cellRegistry, _this.sourceObject);
        if (sourceIndex < 0 || sourceIndex >= container.items.Length) return false;

        Item draggedItem = container.items[sourceIndex];
        if (draggedItem == null || draggedItem.Type != slotIndex + 1) return false;

        // 交换装备
        Item oldEquipped = _this.equippedItems[slotIndex];
        _this.equippedItems[slotIndex] = draggedItem;
        背包初始化.设置格子(_this, sourceIndex, oldEquipped);

        // 清理装备槽旧物体
        var slot = _this.equipmentSlots[slotIndex];
        for (int c = slot.childCount - 1; c >= 0; c--)
            Object.Destroy(slot.GetChild(c).gameObject);

        // 拖拽物移入装备槽
        _this.sourceItem.transform.SetParent(slot, false);
        _this.sourceItem.transform.localScale = Vector3.one;
        if (_this.sourceItem.transform is RectTransform drt)
            drt.anchoredPosition = Vector2.zero;

        Debug.Log($"[装备] Type={draggedItem.Type} → 槽位{slotIndex}");
        return true;
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

    private static int GetEquipSlotIndex(UIResponder _this, GameObject obj)
    {
        if (obj == null || _this.equipmentSlots == null) return -1;
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt == null) return -1;
        for (int i = 0; i < _this.equipmentSlots.Length; i++)
            if (_this.equipmentSlots[i] == rt) return i;
        return -1;
    }
}
}
