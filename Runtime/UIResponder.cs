using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
/// <summary>
/// ItemManager 唯一入口 — 挂载于 Canvas 下空对象。
///
/// ─── 两大 tag，两路由 ───
///   "Item"      → Item触控（长按/短按/交换/滚Grid/详情）
///   "Container" → Container触控（拖拽移动面板）
///   其他 tag   → 忽略
/// </summary>
public partial class UIResponder : MonoBehaviour,
    IPointerDownHandler,   // A — 手指按下
    IBeginDragHandler,     // B — 开始拖拽
    IDragHandler,          // C — 拖拽中（每帧）
    IPointerUpHandler      // D — 手指抬起
{
    // ════════════════════════════════════════════════════════════
    // A 阶段 — 起手
    // ════════════════════════════════════════════════════════════
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        GameObject clicked = eventData.pointerCurrentRaycast.gameObject;
        currentTag = clicked != null ? clicked.tag : null;

        if (currentTag == "Item")
            Item触控.开始点击(this, eventData);
        else if (currentTag == "Container")
            Container触控.开始拖拽(this, eventData);
        else
            currentTag = null;
    }

    // ════════════════════════════════════════════════════════════
    // B 阶段 — 开始拖拽
    // ════════════════════════════════════════════════════════════
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        if (currentTag == "Item")
            Item触控.开始拖拽(this);
    }

    // ════════════════════════════════════════════════════════════
    // C 阶段 — 拖拽中（每帧）
    // ════════════════════════════════════════════════════════════
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        if (currentTag == "Item")
            Item触控.拖拽中(this, eventData);
        else if (currentTag == "Container")
            Container触控.拖拽中(this, eventData);
    }

    // ════════════════════════════════════════════════════════════
    // D 阶段 — 结算（手指抬起）
    // ════════════════════════════════════════════════════════════
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        if (currentTag == "Item")
            Item触控.结算(this, eventData);
        else if (currentTag == "Container")
            Container触控.结算(this);

        currentTag = null;
    }

}
}
