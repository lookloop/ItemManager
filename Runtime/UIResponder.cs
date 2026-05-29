using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
/// <summary>
/// ItemManager 唯一入口 — 挂载于 Canvas 下空对象。
///
/// 职责：
///   1. 接收 Unity UI 事件（IPointerDown/Up/BeginDrag/Drag）
///   2. 单指锁定 + 死区判定 + 长按计时
///   3. 按「短按/长按 × 拖拽/不拖拽」拆分为 8 条分支
///   4. 每条分支直调对应的静态操作器（A~D）
///
/// 不负责：
///   - 格子生成 → __背包生成器
///   - 数据同步 → __背包初始化
///   - 坐标计算 → __UI坐标转换
/// </summary>
public partial class UIResponder : MonoBehaviour, 
    IPointerDownHandler, 
    IPointerUpHandler, 
    IBeginDragHandler, 
    IDragHandler
{
    // ── A 起手 ──
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        A_开始.Execute(this, eventData);
    }

    // ── B 判定：拖拽开始 + 长按计时器触发 → 进入 B_长按开始 ──
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        var delta = eventData.position - eventData.pressPosition;
        if (delta.magnitude < dragDeadzone) return;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        isDrag = true;
    }

    // ── C 持续：短按→滚动 / 长按→拖拽物品 ──
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        if (isLongPress)
            C_长按拖拽中.Execute(this, eventData);
        else
            C_短按拖拽中.Execute(this, eventData);
    }

    // ── D 结算：4 分支 → D_短按点击/短按拖拽/长按点击/长按拖拽 ──
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

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
