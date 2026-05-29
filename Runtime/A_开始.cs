using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
public static class A_开始
{
    public static void Execute(UIResponder _this, PointerEventData eventData)
    {
        //如果是其他手指骚然，直接返回
        if (eventData.pointerId != 0) return; // 非第一根手指，忽略
        //重置长按状态
        _this.isLongPress = false;
        //隐藏详情面板
        if (_this.Panel != null) _this.Panel.gameObject.SetActive(false);
        //记录当前按下的物体
        _this.sourceObject = eventData.pointerCurrentRaycast.gameObject;
        
        //启动计时器
        _this.timerCoroutine = _this.StartCoroutine(计时器(_this));
        
        // 1. 获取起手时的 Canvas 绝对坐标
        _this.beginPosition = UI坐标转换.获取事件局部坐标(_this.canvas.transform as RectTransform, eventData);
        // 2. 记录 Grid 此时的绝对逻辑坐标
        _this.gridPosition = _this.gridTransform.anchoredPosition;
        // 3. 如果点到的是背包，记录该背包的初始坐标
        if (_this.sourceObject != null && _this.sourceObject.CompareTag("背包"))
        {
            _this.backpackPosition = (_this.sourceObject.transform as RectTransform).anchoredPosition;
        }
    }

    /// <summary>长按计时器</summary>
    public static IEnumerator 计时器(UIResponder _this)
    {
        yield return new WaitForSeconds(_this.timerValue);
        B_长按开始.Execute(_this);
    }
}
}
