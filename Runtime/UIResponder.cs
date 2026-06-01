using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
/// <summary>
/// ItemManager 唯一入口 — 挂载于 Canvas 下空对象。
///
/// ─── 两大 tag ───
///   "Item"      格子（有 Grid 则可滚，无 Grid 则只响应长按/点击）
///   "Container" 面板（只响应短按拖拽移动）
///   其他 tag    ItemManager 不理，各自挂脚本处理
///
/// ─── 四阶段管线 ───
///   A: OnPointerDown   → 起手：记录 sourceObject，启计时器/设拖拽
///   B: OnBeginDrag     → 判定：停计时器，isDrag=true
///   C: OnDrag          → 持续：滚 Grid / 拖面板 / 物品跟随
///   D: OnPointerUp     → 结算：交换/复位/详情
///
/// ─── 正交管线 ───
///   短按拖拽 → Grid / Container 专属（Item 不参与）
///   长按管线 → Item 专属（Grid / Container 不参与）
///   短按点击 → Item 专属
/// </summary>
public partial class UIResponder : MonoBehaviour,
    IPointerDownHandler,   // A — 手指按下
    IPointerUpHandler,     // D — 手指抬起
    IBeginDragHandler,     // B — 开始拖拽
    IDragHandler           // C — 拖拽中（每帧）
{
    // ════════════════════════════════════════════════════════════
    // A 阶段 — 起手：pointerId=0（第一根手指）才响应
    //      Item      → 记录 sourceObject → 启动长按计时器
    //      Container → A_PointerDown 内部设 isDrag=true，不启计时器
    //      其他 tag  → A_PointerDown 不处理，直接忽略
    // ════════════════════════════════════════════════════════════
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // 多指过滤 — 只响应第一根手指，避免混乱
        if (eventData.pointerId != 0) return;

        // 委托 A_PointerDown 做 tag 判定 + 状态初始化
        //   Item:      记录 sourceObject, beginPosition, 往上找 Grid 存 dragTarget+dragStartPos
        //   Container: 记录 sourceObject, beginPosition, dragTarget+dragStartPos, 设 isDrag=true
        //   其他:      sourceObject==null 直接 return
        A_PointerDown.Execute(this, eventData);

        // A_PointerDown 已把 sourceObject 存好，这里读它的 tag
        string tag = sourceObject != null ? sourceObject.tag : null;

        // 只有 Item 需要长按计时器（Container 没有长按概念）
        // 计时器到点 → 调 B_LongPressStart → 设 isLongPress=true → 拾起物品
        if (tag == "Item")
            timerCoroutine = StartCoroutine(ItemLongPressTimer());
    }

    // ════════════════════════════════════════════════════════════
    // Item 长按计时器
    //   yield return new WaitForSeconds(timerValue) 等 0.3 秒
    //   到点 → B_LongPressStart.Execute(this) 拾起物品
    //   如果中途拖拽/松手 → OnBeginDrag/OnPointerUp 中 StopCoroutine 取消
    // ════════════════════════════════════════════════════════════
    public IEnumerator ItemLongPressTimer()
    {
        // WaitForSeconds 用的是 Time.time 减法，不漂移，不掉帧
        yield return new WaitForSeconds(timerValue);
        // 到点：触发长按开始 → 设 isLongPress=true → 拾起物品
        B_LongPressStart.Execute(this);
    }

    // ════════════════════════════════════════════════════════════
    // B 阶段 — 开始拖拽：手指移动超过 Unity 内建阈值触发
    //      停掉计时器 → 设 isDrag=true
    //      后续 OnDrag 根据 isDrag + isLongPress 决定走短按还是长按
    // ════════════════════════════════════════════════════════════
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // 多指过滤
        if (eventData.pointerId != 0) return;

        // 用户开始拖拽了 → 立即取消长按计时器
        // 防止计时器到点后误将本次短按拖拽标记为长按
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);  // 协程遥控器按爆
            timerCoroutine = null;           // 标记遥控器已废
        }

        // 标记进入拖拽状态
        // isDrag=true + isLongPress     → 长按拖拽
        // isDrag=true + !isLongPress    → 短按拖拽
        isDrag = true;
    }

    // ════════════════════════════════════════════════════════════
    // C 阶段 — 拖拽中（每帧调用）
    //
    //   三路分支：
    //     isLongPress           → C_LongPressDrag  物品跟随手指 + 悬停检测
    //     Item + dragTarget≠null → ScrollGrid    滚 Grid
    //     其他（Container）      → DragPanel       拖面板
    //
    //   Item 无 Grid（预制体）→ 短按拖拽无反应，跳过
    // ════════════════════════════════════════════════════════════
    public virtual void OnDrag(PointerEventData eventData)
    {
        // 多指过滤
        if (eventData.pointerId != 0) return;

        // 读起手时记录的 tag（A_PointerDown 存的 sourceObject.tag）
        string tag = sourceObject != null ? sourceObject.tag : null;

        if (isLongPress)
        {
            // 长按拖拽 — Item 专属
            // sourceItem 跟随手指移动 + 射线检测悬停的储物格 → 显示阴影
            C_LongPressDrag.Execute(this, eventData);
        }
        else if (tag == "Item")
        {
            // 短按拖拽 — Item 在 Grid 下才能滚
            // dragTarget 在 A_PointerDown 起手时往上找 Grid 存好了
            if (dragTarget != null)
                C_ShortPressDrag.ScrollGrid(this, eventData);
            // dragTarget == null → 预制体 Item，无 Grid，不滚
        }
        else
        {
            // 短按拖拽 — Container 面板跟随手指移动
            C_ShortPressDrag.DragPanel(this, eventData);
        }
    }

    // ════════════════════════════════════════════════════════════
    // D 阶段 — 结算（手指抬起）
    //
    //   Item + 长按 + 拖拽       → D_LongPressEnd      复位/交换/装备
    //   Item + 长按 + 没拖拽     → （内部看 isDrag：true→交换  false→复位）
    //   Item + 短按 + 没拖拽     → D_ShortPressClick   显示详情面板
    //
    //   Grid/Container 短按拖拽  → 无结算（最后一帧即结尾）
    // ════════════════════════════════════════════════════════════
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        // 多指过滤
        if (eventData.pointerId != 0) return;

        // 松手 → 先取消还在跑的计时器（如果短按松手比 0.3s 快）
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        string tag = sourceObject != null ? sourceObject.tag : null;

        if (isLongPress)
        {
            // ── 长按管线（Item 专属）──
            // 内部看 isDrag：true→交换/装备  false→复位
            D_LongPressEnd.Execute(this, eventData);
        }
        else if (tag == "Item" && !isDrag)
        {
            // ── 短按点击（Item 专属）──
            // 单纯点击格子 → 弹出详情面板
            D_ShortPressClick.Execute(this, eventData);
        }
        // else: Grid/Container 短按拖拽 — 无结算
        //   滚动在最后一帧已到位，面板在最后一帧已到位

        // 结算完成，重置拖拽标记
        isDrag = false;
    }

    // ════════════════════════════════════════════════════════════
    // 工具方法
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// 往上遍历父节点，检查是否存在指定 tag
    /// </summary>
    public static bool HasParentTag(Transform child, string tag)
    {
        Transform t = child;
        while (t != null)
        {
            if (t.CompareTag(tag)) return true;
            t = t.parent;
        }
        return false;
    }

    /// <summary>
    /// 从子级往上找到 Container，在 containers 列表中匹配返回 ContainerData
    /// 用于 D_ShortPressClick / D_LongPressEnd 获取 items 数组
    /// </summary>
    public ContainerData GetContainerData(Transform child)
    {
        if (containers == null) return null;
        Transform t = child;
        while (t != null)
        {
            if (t.CompareTag("Container"))
            {
                var rt = t as RectTransform;
                foreach (var cd in containers)
                    if (cd.container == rt) return cd;
                return null;
            }
            t = t.parent;
        }
        return null;
    }

    /// <summary>
    /// 清空拖拽状态 — 隐藏阴影 + 释放引用
    /// D_LongPressEnd 结算后调用
    /// </summary>
    public void ClearDragState()
    {
        if (shadowItem != null)
        {
            shadowItem.SetActive(false);                                              // 隐藏阴影
            shadowItem.transform.SetParent(canvas != null ? canvas.transform : transform, false); // 移回 Canvas
        }
        sourceObject = null;   // 清除起手对象引用
        targetObject = null;   // 清除目标格子引用
        sourceItem   = null;   // 清除拖拽物引用
    }
}
}
