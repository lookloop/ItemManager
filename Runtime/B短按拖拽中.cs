using UnityEngine;
using UnityEngine.EventSystems;

public static class 短按拖拽中
{
    public static void Execute(UIResponder _this, PointerEventData eventData)
    {
        
        Debug.Log("UI 短按拖拽: " + _this.gameObject.name);
        _this.endPosition = UI坐标转换.获取事件局部坐标(_this.canvas.transform as RectTransform, eventData);
        // 判断起手点到的物体，是不是 Grid 的子节点（或者就是 Grid 本身）
        if (!_this.sourceObject.transform.IsChildOf(_this.gridTransform))
        {
            // 如果不是 Grid 里的东西，检测是否是标签为“背包”的物体
            if (_this.sourceObject.CompareTag("背包"))
            {
                拖拽背包面板(_this, eventData);
            }
            return; // 既不是 Grid 里的东西，也不是背包（或者已经处理了背包拖拽），直接返回
        }
        
        // 3. 计算“当前手指位置”与“起手位置”之间的总差值 (绝对差值)
        Vector2 totalDelta = _this.endPosition - _this.beginPosition;
        //在这里获取mask的高度。和grid的高度，相互减掉，看还差多少可以滑动
        float maskHeight = _this.maskTransform.rect.height;
        float gridHeight = _this.gridTransform.rect.height;
        float diff = gridHeight - maskHeight;
        if(diff < 0)
        {
            //设置为0
            diff = 0;
        }



        if(totalDelta.y + _this.gridPosition.y > diff)
        {
            _this.gridTransform.anchoredPosition = new Vector2(0, diff);
        }
        else
        {
            _this.gridTransform.anchoredPosition = _this.gridPosition + new Vector2(0, totalDelta.y + _this.gridPosition.y < 0 ? 0 - _this.gridPosition.y : totalDelta.y);
        }
    }

    private static void 拖拽背包面板(UIResponder _this, PointerEventData eventData)
    {
        if (_this.sourceObject == null) return;
        // 计算当前手指位置与起手位置的xy差值
        Vector2 totalDelta = _this.endPosition - _this.beginPosition;
        // 对xy进行偏移，移动当前点击到的背包面板
        (_this.sourceObject.transform as RectTransform).anchoredPosition = _this.backpackPosition + totalDelta;
    }
}
