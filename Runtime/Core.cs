using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
/// <summary>
/// ItemManager 唯一入口 — 挂载于 Canvas 下空对象。
///
/// ─── 两大 tag，两路由 ───
///   "Item"      → Item触控（长按/短按/交换/滚Grid/详情）
///   "Container" → Container触控（拖拽移动面板）
///   其他 tag   → 忽略
///
/// ─── 拖拽判定：距离判定，非 BeginDrag 事件 ───
///   Item/Container 各自在 OnDrag 中对比起手坐标与当前坐标，
///   不一致则视为拖拽（isDrag），不再依赖 Unity IBeginDragHandler。
/// </summary>
public partial class Core : MonoBehaviour,
    IPointerDownHandler,   // A — 手指按下
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
        PointerDownTag = clicked != null ? clicked.tag : null;

        if (PointerDownTag == "Item")
            ItemTouch.BeginClick(this, eventData);
        else if (PointerDownTag == "Container")
            ContainerTouch.BeginDrag(this, eventData);
        else
            PointerDownTag = null;
    }

    // ════════════════════════════════════════════════════════════
    // C 阶段 — 拖拽中（每帧）
    // ════════════════════════════════════════════════════════════
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        if (PointerDownTag == "Item")
            ItemTouch.OnDrag(this, eventData);
        else if (PointerDownTag == "Container")
            ContainerTouch.OnDrag(this, eventData);
    }

    // ════════════════════════════════════════════════════════════
    // D 阶段 — 结算（手指抬起）
    // ════════════════════════════════════════════════════════════
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        if (PointerDownTag == "Item")
            ItemTouch.EndDrag(this, eventData);
        else if (PointerDownTag == "Container")
            ContainerTouch.EndDrag(this);

        PointerDownTag = null;
    }

}
}