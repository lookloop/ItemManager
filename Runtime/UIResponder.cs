using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class UIResponder : MonoBehaviour, 
    IPointerDownHandler, 
    IPointerUpHandler, 
    IBeginDragHandler, 
    IDragHandler
{
    void Awake()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        A_开始.Execute(this, eventData);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != onlyResponder) return;

        var delta = eventData.position - eventData.pressPosition;
        if (delta.magnitude < dragDeadzone) return;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        isDrag = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != onlyResponder) return;

        if (isLongPress)
            C_长按拖拽中.Execute(this, eventData);
        else
            C_短按拖拽中.Execute(this, eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != onlyResponder) return;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        if (isLongPress)
        {
            if (isDrag)
                D_长按拖拽结束.Execute(this, eventData);
            else
                D_长按点击结束.Execute(this, eventData);
        }
        else
        {
            if (isDrag)
                D_短按拖拽结束.Execute(this, eventData);
            else
                D_短按点击结束.Execute(this, eventData);
        }

        isDrag = false;
        onlyResponder = -1;
    }

    public void ClearDragState()
    {
        if (shadowItem != null)
        {
            shadowItem.SetActive(false);
            shadowItem.transform.SetParent(canvas != null ? canvas.transform : transform, false);
        }
        sourceObject = null;
        targetObject = null;
        sourceItem = null;
    }
}
}
