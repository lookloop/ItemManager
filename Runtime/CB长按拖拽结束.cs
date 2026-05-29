using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
public static class 长按拖拽结束
{
    public static void Execute(UIResponder _this, PointerEventData eventData)
    {
        Debug.Log("执行程序：长按拖拽结算 (Long Drag End)");

        // 1. 装备槽检测：松手位置是否为装备槽
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        int equipSlotIndex = GetEquipSlotIndex(_this, dropTarget);
        if (equipSlotIndex >= 0 && _this.sourceObject != null)
        {
            int sourceIndex = System.Array.IndexOf(_this.cellRegistry, _this.sourceObject);
            if (sourceIndex >= 0 && sourceIndex < _this.items.Length)
            {
                Item draggedItem = _this.items[sourceIndex];
                int expectedType = equipSlotIndex + 1; // 槽0=头盔(Type1), 槽1=身甲(Type2), 槽2=护手(Type3), 槽3=护腿(Type4)

                if (draggedItem != null && draggedItem.Type == expectedType)
                {
                    // 数据交换
                    Item oldEquipped = _this.equippedItems[equipSlotIndex];
                    _this.equippedItems[equipSlotIndex] = draggedItem;
                    背包初始化.设置格子(_this, sourceIndex, oldEquipped);

                    // 清理装备槽旧 Item
                    for (int c = _this.equipmentSlots[equipSlotIndex].childCount - 1; c >= 0; c--)
                        Object.Destroy(_this.equipmentSlots[equipSlotIndex].GetChild(c).gameObject);

                    // 拖拽物移到装备槽
                    _this.sourceItem.transform.SetParent(_this.equipmentSlots[equipSlotIndex], false);
                    _this.sourceItem.transform.localScale = Vector3.one;
                    if (_this.sourceItem.transform is RectTransform drt) drt.anchoredPosition = Vector2.zero;

                    // TODO: 3D换装 — 外部系统直接读 equippedItems[] 即可

                    Debug.Log($"[装备槽] 装备成功: Type={draggedItem.Type} → 槽位{equipSlotIndex}");
                }
                else
                {
                    // Type 不匹配，复位
                    Debug.Log($"[装备槽] Type不匹配: 拖拽Type={draggedItem?.Type}, 槽位期望Type={expectedType}");
                    ResetSourceItem(_this);
                }
            }
            else
            {
                ResetSourceItem(_this);
            }

            // 装备槽结算完成，清空并返回
            _this.ClearDragState();
            return;
        }

        // 3. 检查交互条件：目标格子存在，且不是起始格子
        // （既然能进入长按拖拽结算，说明 sourceObject 和 sourceItem 必然存在）
        if (_this.targetObject != null && _this.targetObject != _this.sourceObject)
        {
            // 使用 System.Array.IndexOf 获取对象在 cellRegistry 中的索引
            int sourceIndex = System.Array.IndexOf(_this.cellRegistry, _this.sourceObject);
            int targetIndex = System.Array.IndexOf(_this.cellRegistry, _this.targetObject);

            if (sourceIndex >= 0 && sourceIndex < _this.items.Length && 
                targetIndex >= 0 && targetIndex < _this.items.Length)
            {
                // 交换
                var srcItem = _this.items[sourceIndex];
                var dstItem = _this.items[targetIndex];
                背包初始化.设置格子(_this, sourceIndex, dstItem);
                背包初始化.设置格子(_this, targetIndex, srcItem);

                // 销毁漂浮在 Canvas 上的旧拖拽物
                if (_this.sourceItem != null) Object.Destroy(_this.sourceItem);

                Debug.Log($"交换完成：索引 {sourceIndex} 与索引 {targetIndex} 交换");
            }
            else
            {
                // 索引越界或未找到，复位
                ResetSourceItem(_this);
            }
        }
        else
        {
            // 目标格子为空，或者是起始格子，复位
            ResetSourceItem(_this);
        }


        // 5. 结算完成后，清空临时记录
        _this.ClearDragState();
    }

    // 辅助方法：检测松手 GameObject 是否为装备槽，返回槽索引(-1表示不是)
    private static int GetEquipSlotIndex(UIResponder _this, GameObject obj)
    {
        if (obj == null || _this.equipmentSlots == null) return -1;
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt == null) return -1;
        for (int i = 0; i < _this.equipmentSlots.Length; i++)
        {
            if (_this.equipmentSlots[i] == rt)
                return i;
        }
        return -1;
    }

    // 辅助方法：将拖拽物复位回起始格子
    private static void ResetSourceItem(UIResponder _this)
    {
        if (_this.sourceItem != null && _this.sourceObject != null)
        {
            _this.sourceItem.transform.SetParent(_this.sourceObject.transform, false);
            _this.sourceItem.transform.localScale = Vector3.one; // 恢复局部缩放
            if (_this.sourceItem.transform is RectTransform rt) rt.anchoredPosition = Vector2.zero;
        }
    }
}
}
