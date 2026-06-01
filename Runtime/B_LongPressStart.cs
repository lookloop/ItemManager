using UnityEngine;

namespace Lookloop.ItemManager
{
/// <summary>B 长按开始 — 计时器到点，设 isLongPress，拾起物品到 Canvas</summary>
public static class B_LongPressStart
{
    public static void Execute(UIResponder _this)
    {
        // 长按生效，改变变量
        _this.isLongPress = true;
        
        // 如果开始物体是储物格，并且里面有物品（有子物体）
        if (_this.sourceObject != null && _this.sourceObject.transform.childCount > 0)
        {
            // 获取储物格里的实际物品（第一个子物体）并记录为拖拽物
            _this.sourceItem = _this.sourceObject.transform.GetChild(0).gameObject;

            // 将真实物品直接挂载到 Canvas 下，使其脱离格子，准备跟随手指
            _this.sourceItem.transform.SetParent(_this.canvas.transform, false);
            
            // 缩放物品，保持与储物格一致的视觉大小
            _this.sourceItem.transform.localScale = new Vector3(_this.cellWidth / 10f, _this.cellWidth / 10f, 1f);
            
            // 物品直接跟随点击点
            (_this.sourceItem.transform as RectTransform).anchoredPosition = _this.beginPosition;
            
            // 确保在最上层显示
            _this.sourceItem.transform.SetAsLastSibling();
        }
        else
        {
            // 如果开始物体不是储物格（或者里面没东西），那么直接返回，长按不生效
            _this.isLongPress = false;
            return;
        }
    }
}
}
