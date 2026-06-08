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
    [HideInInspector] public RectTransform  hitRect;
    [HideInInspector] public ContainerMod  hitContainerMod;
    [HideInInspector] public RectTransform  lastDetail;

    Coroutine _holdTimer;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;


        _holdTimer = StartCoroutine(HoldTimerRoutine());

        hitRect = eventData.pointerCurrentRaycast.gameObject?.transform as RectTransform;
        hitContainerMod = null;
        atTag = hitRect?.tag;
        if (hitRect != null)
        {
            Transform t = hitRect;
            while (t != null && !t.CompareTag("Container"))
                t = t.parent;

            if (t != null)
            {
                foreach (var m in containers)
                {
                    if (m.container == t)
                        { hitContainerMod = m; break; }
                }
            }
        }

        switch (atTag)
        {
            case "Cell":
                CellTouch.BeginTouch(this, eventData);
                break;
            case "Container":
                ContainerTouch.BeginDrag(this, eventData);
                break;
            case "TurnPage":
                Debug.Log("开局turnpage");
                break;
            default:
                // 点击空白区域 → 隐藏 detail
                HideDetail();
                break;
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        isDrag = true;

        switch (atTag)
        {
            case "Cell":
                CellTouch.OnDrag(this, eventData);
                break;
            case "Container":
                ContainerTouch.OnDrag(this, eventData);
                break;
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        switch (atTag)
        {
            case "Cell":
                CellTouch.EndTouch(this);
                break;
            case "Container":
                ContainerTouch.EndDrag(this);
                break;
            case "TurnPage":
                TurnPageTouch.Click(this, eventData);
                break;
        }

        Reset();
    }

    IEnumerator HoldTimerRoutine()
    {
        bool triggered = false;
        while (true)
        {
            holdTime += Time.deltaTime;

            if (!triggered && holdTime > 0.3f && atTag == "Cell")
            {
                triggered = true;
                CellTouch.LongPress(this);
            }

            if (triggered)
                CellTouch_Drag.Update(this);

            yield return null;
        }
    }

    public void ShowDetail(ContainerMod mod)
    {
        // 隐藏上一个
        HideDetail();

        if (mod.detail == null || mod.detailFiller == null) return;

        mod.detail.gameObject.SetActive(true);
        int cellIndex = int.Parse(hitRect.name);
        int itemKey = (mod.currentPage - 1) * mod.cells.Length + cellIndex;
        mod.detailFiller.Fill(mod, itemKey);
        lastDetail = mod.detail;
    }

    public void HideDetail()
    {
        if (lastDetail != null)
        {
            lastDetail.gameObject.SetActive(false);
            lastDetail = null;
        }
    }

    public void Reset()
    {
        StopCoroutine(_holdTimer);
        _holdTimer = null;
        isDrag = false;
        holdTime = 0f;
        atTag = null;
        hitRect = null;
        hitContainerMod = null;
    }
}
}
