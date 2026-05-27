using UnityEngine;
using UnityEngine.EventSystems;

public static class 长按点击结束
{
    public static void Execute(UIResponder _this, PointerEventData eventData)
    {
        Debug.Log("执行程序：长按不拖拽点击结算 (Long Press Click)");
        
        // 长按开始时物品被拿到了 Canvas 下，如果没有拖拽直接松手，需要将其复位回原格子
        ResetSourceItem(_this);

        // 结算完成后，清空临时记录
        _this.sourceObject = null;
        _this.targetObject = null;
        _this.sourceItem = null;
        _this.targetItem = null;
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
