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
        开始点击.Execute(this, eventData);
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
            长按拖拽中.Execute(this, eventData);
        else
            短按拖拽中.Execute(this, eventData);
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
                长按拖拽结束.Execute(this, eventData);
            else
                长按点击结束.Execute(this, eventData);
        }
        else
        {
            if (isDrag)
                短按拖拽结束.Execute(this, eventData);
            else
                短按点击结束.Execute(this, eventData);
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
