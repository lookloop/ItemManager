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
    [HideInInspector] public Container sourceContainer;
    [HideInInspector] public bool isDrag = false;
    [HideInInspector] public Vector2 sourcePos;
    [HideInInspector] public Vector2 onPos;
    [HideInInspector] public bool isLongPress;
    [HideInInspector] public Coroutine longPressCoroutine;
    [HideInInspector] public PointerEventData eventData;
    [HideInInspector] public RectTransform targetRect;
    [HideInInspector] public Container targetContainer;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;
        Begin(eventData);
        

        switch (sourceRect.gameObject.tag)
        {
            case "Container":
                TouchContainer.On(this);
                break;
            case "Cell":
                TouchCell.On(this);
                break;
        }


    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;
        isDrag = true;

        switch (sourceRect.gameObject.tag)
        {
            case "Container":
                TouchContainer.OnDrag(this);
                break;
            case "Cell":
                TouchCell.OnDrag(this);
                break;
        }


    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;
        

        switch (sourceRect.gameObject.tag)
        {
            case "TurnPage":
                TouchTurnPage.End(this);
                break;
        }

        Reset();

    }








    
    public virtual void Begin(PointerEventData eventData)
        {
            this.eventData = eventData;
            sourceRect = eventData.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>();
            sourcePos = sourceRect.anchoredPosition;

            // 沿父级向上找 tag=Container，通过 name 定位 container
            var t = sourceRect.parent;
            while (t != null)
            {
                if (t.CompareTag("Container"))
                {
                    if (int.TryParse(t.name, out int index) && index < containers.Length)
                        sourceContainer = containers[index];
                    break;
                }
                t = t.parent;
            }
        }

    public virtual void Reset()
        {
            sourceRect = null;
            sourceContainer = null;
            isDrag = false;
            sourcePos = Vector2.zero;
            onPos = Vector2.zero;

            isLongPress = false;
            if (longPressCoroutine != null)
            {
                StopCoroutine(longPressCoroutine);
                longPressCoroutine = null;
            }
            eventData = null;

            targetRect = null;
            targetContainer = null;
            lastTurnTime = 0f;

            OtherTool.dragRect.gameObject.SetActive(false);
            OtherTool.Shadow.gameObject.SetActive(false);
        }


    public IEnumerator LongPressTimer()
    {
        yield return new WaitForSeconds(pressTime);
        isLongPress = true;
        TouchItem.ExtractItem(this);

        while (true)
        {
            TouchMask.ScrollPage(this);
            TouchMask.TurnPage(this);
            yield return null;
        }
    }




}
}
