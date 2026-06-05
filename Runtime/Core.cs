using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public partial class Core : MonoBehaviour,
    IPointerDownHandler,   // A — 手指按下
    IDragHandler,          // C — 拖拽中（每帧）
    IPointerUpHandler      // D — 手指抬起
{

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        GameObject clicked = eventData.pointerCurrentRaycast.gameObject;
        PointerDownTag = clicked != null ? clicked.tag : null;
        PointerDragged = false;

        if (PointerDownTag == "Cell")
            //临时重构：先输出文本，后续再调用 ItemTouch.BeginDrag(this, eventData);
            Debug.Log("临时重构");
        else if (PointerDownTag == "Container")
            ContainerTouch.BeginDrag(this, eventData);
        else if (PointerDownTag == "TurnPage")
        {
            // 不拖拽，PointerUp 时结算
        }
        else
            PointerDownTag = null;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        PointerDragged = true;

        if (PointerDownTag == "Cell")
            //临时重构：先输出文本，后续再调用 ItemTouch.OnDrag(this, eventData);
            Debug.Log("临时重构");
        else if (PointerDownTag == "Container")
            ContainerTouch.OnDrag(this, eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        if (PointerDownTag == "Cell")
            //输出文本
            Debug.Log("临时重构") ;
        else if (PointerDownTag == "Container")
            ContainerTouch.EndDrag(this);
        else if (PointerDownTag == "TurnPage" && !PointerDragged)
            TurnPageTouch.Click(this, eventData);

        PointerDownTag = null;
    }

}
}
