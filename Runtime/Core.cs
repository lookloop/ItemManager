using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public partial class Core : MonoBehaviour,
    IPointerDownHandler,   // A — 手指按下
    IDragHandler,          // C — 拖拽中（每帧）
    IPointerUpHandler      // D — 手指抬起
{

    [HideInInspector] public string atTag;
    [HideInInspector] public bool isDrag;

    public virtual void OnPointerDown(PointerEventData eventData)
    {


        if (eventData.pointerId != 0) return;

        GameObject clicked = eventData.pointerCurrentRaycast.gameObject;
        atTag = clicked != null ? clicked.tag : null;
        isDrag = false;

        if (atTag == "Cell")
            //临时重构：先输出文本，后续再调用 CellTouch.BeginDrag(this, eventData);
            Debug.Log("临时重构");
        else if (atTag == "Container")
            ContainerTouch.BeginDrag(this, eventData);
        else if (atTag == "TurnPage")
        {
            // 不拖拽，PointerUp 时结算
        }
        else
            atTag = null;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        isDrag = true;

        if (atTag == "Cell")
            //临时重构：先输出文本，后续再调用 CellTouch.OnDrag(this, eventData);
            Debug.Log("临时重构");
        else if (atTag == "Container")
            ContainerTouch.OnDrag(this, eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        if (atTag == "Cell")
            //输出文本
            Debug.Log("临时重构") ;
        else if (atTag == "Container")
            ContainerTouch.EndDrag(this);
        else if (atTag == "TurnPage" && !isDrag)
            TurnPageTouch.Click(this, eventData);

        atTag = null;
    }

}
}
