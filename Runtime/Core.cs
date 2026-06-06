using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public partial class Core : MonoBehaviour,
    IPointerDownHandler,   // A — 手指按下
    IDragHandler,          // C — 拖拽中（每帧）
    IPointerUpHandler      // D — 手指抬起
{

    [HideInInspector] public string Tag;
    [HideInInspector] public bool isDrag;

    public virtual void OnPointerDown(PointerEventData eventData)
    {


        if (eventData.pointerId != 0) return;

        GameObject clicked = eventData.pointerCurrentRaycast.gameObject;
        Tag = clicked != null ? clicked.tag : null;
        isDrag = false;

        if (Tag == "Cell")
            //临时重构：先输出文本，后续再调用 CellTouch.BeginDrag(this, eventData);
            Debug.Log("临时重构");
        else if (Tag == "Container")
            ContainerTouch.BeginDrag(this, eventData);
        else if (Tag == "TurnPage")
        {
            // 不拖拽，PointerUp 时结算
        }
        else
            Tag = null;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        isDrag = true;

        if (Tag == "Cell")
            //临时重构：先输出文本，后续再调用 CellTouch.OnDrag(this, eventData);
            Debug.Log("临时重构");
        else if (Tag == "Container")
            ContainerTouch.OnDrag(this, eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        if (Tag == "Cell")
            //输出文本
            Debug.Log("临时重构") ;
        else if (Tag == "Container")
            ContainerTouch.EndDrag(this);
        else if (Tag == "TurnPage" && !isDrag)
            TurnPageTouch.Click(this, eventData);

        Tag = null;
    }

}
}
