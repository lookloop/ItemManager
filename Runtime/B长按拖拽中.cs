using UnityEngine;
using UnityEngine.EventSystems;

public static class 长按拖拽中
{
    public static void Execute(UIResponder _this, PointerEventData eventData)
    {
        Debug.Log("UI 长按拖拽: " + _this.gameObject.name);
        
        // 1. sourceItem 一直跟随落点（eventData 转化为 canvas 的局部坐标）
        if (_this.sourceItem != null)
        {
            RectTransform sourceRT = _this.sourceItem.transform as RectTransform;
            if (sourceRT != null)
            {
                sourceRT.anchoredPosition = UI坐标转换.获取事件局部坐标(_this.canvas.transform as RectTransform, eventData);
            }
        }

        // 2. 射线检测储物格
        GameObject hoverObj = eventData.pointerCurrentRaycast.gameObject;

        // 只要是储物格就行
        if (hoverObj != null && hoverObj.CompareTag("储物格"))
        {
            // 如果悬停的格子发生了变化
            if (_this.targetObject != hoverObj)
            {
                _this.targetObject = hoverObj;
                
                // 将阴影对象（shadowItem）放到当前悬停的格子里
                if (_this.shadowItem != null)
                {
                    _this.shadowItem.SetActive(true);
                    _this.shadowItem.transform.SetParent(hoverObj.transform, false);
                    (_this.shadowItem.transform as RectTransform).anchoredPosition = Vector2.zero;
                    
                    // 保证阴影在最上层（盖住格子里的原有物品）
                    _this.shadowItem.transform.SetAsLastSibling(); 
                }
            }
        }
        else
        {
            // 如果没有悬停在储物格上
            if (_this.targetObject != null)
            {
                _this.targetObject = null;
                if (_this.shadowItem != null)
                {
                    _this.shadowItem.SetActive(false);
                    // 移回 Canvas，避免留在某个格子里
                    _this.shadowItem.transform.SetParent(_this.canvas.transform, false);
                }
            }
        }
    }
}
