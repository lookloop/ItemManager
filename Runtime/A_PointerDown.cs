using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
/// <summary>A 起手 — 记录 sourceObject，判定 tag 分支，启计时器/设拖拽</summary>
public static class A_PointerDown
{
    public static void Execute(UIResponder _this, PointerEventData eventData)
    {
        _this.isLongPress = false;
        _this.isDrag = false;
        if (_this.Panel != null) _this.Panel.gameObject.SetActive(false);

        _this.sourceObject = eventData.pointerCurrentRaycast.gameObject;
        if (_this.sourceObject == null) return;

        string tag = _this.sourceObject.tag;
        _this.beginPosition = UI坐标转换.获取事件局部坐标(_this.canvas.transform as RectTransform, eventData);

        if (tag == "Container")
        {
            _this.dragTarget = _this.sourceObject.GetComponent<RectTransform>();
            _this.dragStartPos = _this.dragTarget.anchoredPosition;
            _this.isDrag = true;
        }
        else if (tag == "Item")
        {
            // 往上找 Grid，存引用和初始位置
            Transform t = _this.sourceObject.transform;
            while (t != null && !t.CompareTag("Grid"))
                t = t.parent;
            if (t != null)
            {
                _this.dragTarget = t as RectTransform;
                _this.dragStartPos = _this.dragTarget.anchoredPosition;
            }
        }
    }
}
}
