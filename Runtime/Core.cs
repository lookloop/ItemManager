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

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;
        Begin(eventData);
        

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
        isDrag = true;

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
        

        switch (sourceRect.gameObject.tag)
        {
            case "TurnPage":
                TouchTurnPage.End(this, eventData);
                break;
        }

        Reset();

    }








    
    public virtual void Begin(PointerEventData eventData)
        {
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
        }

  




}
}
