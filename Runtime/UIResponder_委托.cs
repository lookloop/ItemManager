using UnityEngine;
using UnityEngine.EventSystems;

public partial class UIResponder
{
    // ── 8个结算分支委托 ──────────────────────────────

    /// <summary>A开始点击 — 手指按下</summary>
    public System.Action<PointerEventData> OnTapBegin;

    /// <summary>A长按开始 — 长按判定生效</summary>
    public System.Action OnLongPressBegin;

    /// <summary>B短按拖拽中 — 短按滑动</summary>
    public System.Action<PointerEventData> OnShortDragging;

    /// <summary>B长按拖拽中 — 长按拖拽物品</summary>
    public System.Action<PointerEventData> OnLongDragging;

    /// <summary>CA短按点击结束 — 短按抬起（无拖拽）</summary>
    public System.Action<PointerEventData> OnShortClickEnd;

    /// <summary>CA长按点击结束 — 长按抬起（无拖拽）</summary>
    public System.Action<PointerEventData> OnLongClickEnd;

    /// <summary>CB短按拖拽结束 — 短按滑动松手</summary>
    public System.Action<PointerEventData> OnShortDragEnd;

    /// <summary>CB长按拖拽结束 — 长按拖拽松手</summary>
    public System.Action<PointerEventData> OnLongDragEnd;

    /// <summary>装备请求: (accountId, equipTypeIndex, key3D)</summary>
    public System.Action<long, int, int> OnEquipRequest;

    // ── 注册委托列表 ──────────────────────────────

    public void 注册委托列表()
    {
        OnTapBegin       += (e) => 开始点击.Execute(this, e);
        OnLongPressBegin += () => 长按开始.Execute(this);
        OnShortDragging += (e) => 短按拖拽中.Execute(this, e);
        OnLongDragging  += (e) => 长按拖拽中.Execute(this, e);
        OnShortClickEnd += (e) => 短按点击结束.Execute(this, e);
        OnLongClickEnd  += (e) => 长按点击结束.Execute(this, e);
        OnShortDragEnd  += (e) => 短按拖拽结束.Execute(this, e);
        OnLongDragEnd   += (e) => 长按拖拽结束.Execute(this, e);
    }
}
