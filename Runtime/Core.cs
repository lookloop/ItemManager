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

    [HideInInspector] public RectTransform atRect;
    [HideInInspector] public bool isDrag;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;


        switch (atRect.gameObject.tag)
        {
            case "Container":
                TouchContainer.On(this, eventData);
                break;
        }


    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;


    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

    }

  




}
}
