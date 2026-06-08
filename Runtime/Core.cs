using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public partial class Core : MonoBehaviour,
    IPointerDownHandler,   // A — 手指按下
    IDragHandler,          // C — 拖拽中（每帧）
    IPointerUpHandler      // D — 手指抬起
{
    //有关触控的字段

    [HideInInspector] public RectTransform sourceRect;
    [HideInInspector] public bool isDrag;
    [HideInInspector] public Vector2 sourcePos;
    [HideInInspector] public Vector2 onPos;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;
        sourceRect = eventData.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>();
        sourcePos = sourceRect.anchoredPosition;

        switch (sourceRect.gameObject.tag)
        {
            case "Container":
                TouchContainer.On(this, eventData);
                break;
        }


    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        switch (sourceRect.gameObject.tag)
        {
            case "Container":
                TouchContainer.OnDrag(this, eventData);
                break;
        }


    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        
        Reset();

    }

    public virtual void Reset()
        {
            //设置全null
            sourceRect = null;
            isDrag = false;
            sourcePos = Vector2.zero;
            onPos = Vector2.zero;
        }

  




}
}
