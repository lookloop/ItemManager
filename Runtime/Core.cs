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

    [HideInInspector] public string atTag;
    [HideInInspector] public bool isDrag;
    [HideInInspector] public float holdTime;

    Coroutine _holdTimer;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //初始化
        if (eventData.pointerId != 0) return;
        _holdTimer = StartCoroutine(HoldTimerRoutine());
        //初始化

        GameObject clicked = eventData.pointerCurrentRaycast.gameObject;
        atTag = clicked?.tag;
        
        if (atTag == "Cell")
            Debug.Log("开局cell");
        if (atTag == "Container")
            ContainerTouch.BeginDrag(this, eventData);
        if (atTag == "TurnPage")
            Debug.Log("开局turnpage");
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        //初始化
        if (eventData.pointerId != 0) return;
        isDrag = true;
        //初始化



        if (atTag == "Cell")
            Debug.Log("临时重构");
        if (atTag == "Container")
            ContainerTouch.OnDrag(this, eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //初始化
        if (eventData.pointerId != 0) return;
        //初始化
        


        if (atTag == "Cell")
            Debug.Log("收起的cell") ;
        if (atTag == "Container")
            ContainerTouch.EndDrag(this);
        if (atTag == "TurnPage")
            TurnPageTouch.Click(this, eventData);

        //使用重置方法
        Reset();
        //使用重置方法
    }

    IEnumerator HoldTimerRoutine()
    {
        while (true)
        {
            holdTime += Time.deltaTime;
            yield return null;
        }
    }
    public void Reset()
    {
        StopCoroutine(_holdTimer);
        _holdTimer = null;
        isDrag = false;
        holdTime = 0f;
        atTag = null;
    }
}
}
