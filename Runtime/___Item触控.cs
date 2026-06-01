using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Lookloop.ItemManager
{
/// <summary>
/// Item 触控总成 — 黑盒处理所有 Item 交互。
/// UIResponder 只做路由：Item → 调这 4 个方法；Container → 自理。
///
/// ─── 3 个核心字段 ───
///   source       来源 cell
///   target       目标 cell
///   itemDragging 跟随手指的 Item
///
/// ─── 4 个公开方法 ───
///   开始点击  → 记录 source，启计时器
///   开始拖拽  → 停计时器，标 isDrag
///   拖拽中    → 内部判断长按/短按：物品跟随 或 滚Grid
///   结算      → 内部判断：交换/复位/详情面板
/// </summary>
public static class Item触控
{
    // ════════════════════════════════════════════════════════════
    // 3 个核心字段
    // ════════════════════════════════════════════════════════════
    public static GameObject source;        // 来源 cell
    public static GameObject target;        // 目标 cell（悬停到的格子）
    public static GameObject itemDragging;  // 跟随手指的漂浮 Item

    // ════════════════════════════════════════════════════════════
    // 内部状态（外部不可见）
    // ════════════════════════════════════════════════════════════
    static bool       isLongPress;
    static bool       isDrag;
    static Vector2    beginPosition;
    static Coroutine  timerCoroutine;
    static RectTransform gridTarget;    // 滚 Grid 用
    static Vector2    gridStartPos;

    // ════════════════════════════════════════════════════════════
    // 1. 开始点击 — A 阶段
    // ════════════════════════════════════════════════════════════
    public static void 开始点击(UIResponder _this, PointerEventData eventData)
    {
        // 重置状态
        isLongPress = false;
        isDrag      = false;
        if (_this.Panel != null) _this.Panel.gameObject.SetActive(false);

        source = eventData.pointerCurrentRaycast.gameObject;
        if (source == null) return;

        beginPosition = UI坐标转换.获取事件局部坐标(_this.canvas.transform as RectTransform, eventData);

        // 往上找 Grid（短按拖拽滚 Grid 用）
        Transform t = source.transform;
        while (t != null && !t.CompareTag("Grid"))
            t = t.parent;
        if (t != null)
        {
            gridTarget  = t as RectTransform;
            gridStartPos = gridTarget.anchoredPosition;
        }
        else
        {
            gridTarget  = null;
            gridStartPos = Vector2.zero;
        }

        // 启计时器 — 到点自动变长按
        if (timerCoroutine != null)
            _this.StopCoroutine(timerCoroutine);
        timerCoroutine = _this.StartCoroutine(计时器(_this));
    }

    static IEnumerator 计时器(UIResponder _this)
    {
        yield return new WaitForSeconds(_this.timerValue);

        // ── 长按成功：拾起物品 ──
        isLongPress = true;

        if (source != null && source.transform.childCount > 0)
        {
            itemDragging = source.transform.GetChild(0).gameObject;
            itemDragging.transform.SetParent(_this.canvas.transform, false);
            itemDragging.transform.localScale = new Vector3(_this.cellWidth / 10f, _this.cellWidth / 10f, 1f);
            (itemDragging.transform as RectTransform).anchoredPosition = beginPosition;
            itemDragging.transform.SetAsLastSibling();
        }
        else
        {
            isLongPress = false;
        }
    }

    // ════════════════════════════════════════════════════════════
    // 2. 开始拖拽 — B 阶段
    // ════════════════════════════════════════════════════════════
    public static void 开始拖拽(UIResponder _this)
    {
        if (timerCoroutine != null)
        {
            _this.StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        isDrag = true;
    }

    // ════════════════════════════════════════════════════════════
    // 3. 拖拽中 — C 阶段（每帧）
    // ════════════════════════════════════════════════════════════
    public static void 拖拽中(UIResponder _this, PointerEventData eventData)
    {
        if (isLongPress)
            长按跟随(_this, eventData);
        else if (gridTarget != null)
            滚Grid(_this, eventData);
    }

    // ════════════════════════════════════════════════════════════
    // 4. 结算 — D 阶段（手指抬起）
    // ════════════════════════════════════════════════════════════
    public static void 结算(UIResponder _this, PointerEventData eventData)
    {
        // 先停计时器
        if (timerCoroutine != null)
        {
            _this.StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        if (isLongPress)
        {
            if (isDrag)  交换(_this);
            else         复位(_this);
        }
        else if (!isDrag)
        {
            显示详情(_this, eventData);
        }
        // else: 短按拖拽 — 滚 Grid 最后一帧已到位，无结算

        清理(_this);
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：长按物品跟随手指 + 阴影悬停检测 ──
    // ════════════════════════════════════════════════════════════
    static void 长按跟随(UIResponder _this, PointerEventData eventData)
    {
        Debug.Log("UI 长按拖拽: " + _this.gameObject.name);

        // 物品跟随
        if (itemDragging != null)
        {
            RectTransform rt = itemDragging.transform as RectTransform;
            if (rt != null)
                rt.anchoredPosition = UI坐标转换.获取事件局部坐标(_this.canvas.transform as RectTransform, eventData);
        }

        // 射线检测悬停格子
        GameObject hoverObj = eventData.pointerCurrentRaycast.gameObject;
        if (hoverObj != null && hoverObj.CompareTag("Item"))
        {
            if (target != hoverObj)
            {
                target = hoverObj;
                if (_this.shadowItem != null)
                {
                    _this.shadowItem.SetActive(true);
                    _this.shadowItem.transform.SetParent(hoverObj.transform, false);
                    (_this.shadowItem.transform as RectTransform).anchoredPosition = Vector2.zero;
                    _this.shadowItem.transform.SetAsLastSibling();
                }
            }
        }
        else
        {
            if (target != null)
            {
                target = null;
                if (_this.shadowItem != null)
                {
                    _this.shadowItem.SetActive(false);
                    _this.shadowItem.transform.SetParent(_this.canvas.transform, false);
                }
            }
        }
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：短按滚 Grid（X 锁定，Y 钳位）──
    // ════════════════════════════════════════════════════════════
    static void 滚Grid(UIResponder _this, PointerEventData eventData)
    {
        RectTransform maskRT = gridTarget.parent as RectTransform;
        if (maskRT == null) return;

        Vector2 now = UI坐标转换.获取事件局部坐标(_this.canvas.transform as RectTransform, eventData);
        Vector2 delta = now - beginPosition;

        float maskHeight  = maskRT.rect.height;
        float gridHeight  = gridTarget.rect.height;
        float diff        = Mathf.Max(0, gridHeight - maskHeight);

        float targetY = gridStartPos.y + delta.y;
        targetY = Mathf.Clamp(targetY, 0, diff);

        gridTarget.anchoredPosition = new Vector2(0, targetY);
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：复位 — 物品回原格 ──
    // ════════════════════════════════════════════════════════════
    static void 复位(UIResponder _this)
    {
        if (itemDragging != null && source != null)
        {
            itemDragging.transform.SetParent(source.transform, false);
            itemDragging.transform.localScale = Vector3.one;
            if (itemDragging.transform is RectTransform rt)
                rt.anchoredPosition = Vector2.zero;
        }
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：格子交换 ──
    // ════════════════════════════════════════════════════════════
    static void 交换(UIResponder _this)
    {
        if (!尝试交换(_this))
            复位(_this);
    }

    static bool 尝试交换(UIResponder _this)
    {
        if (target == null || target == source) return false;

        var cont = 取容器数据(_this, source.transform);
        if (cont == null || cont.items == null) return false;

        int srcIdx = System.Array.IndexOf(_this.cellRegistry, source);
        int dstIdx = System.Array.IndexOf(_this.cellRegistry, target);
        if (srcIdx < 0 || dstIdx < 0 || srcIdx >= cont.items.Length || dstIdx >= cont.items.Length)
            return false;

        var srcItem = cont.items[srcIdx];
        var dstItem = cont.items[dstIdx];
        背包初始化.设置格子(_this, srcIdx, dstItem);
        背包初始化.设置格子(_this, dstIdx, srcItem);

        if (itemDragging != null) Object.Destroy(itemDragging);

        Debug.Log($"交换: {srcIdx} ↔ {dstIdx}");
        return true;
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：从子级往上找 Container，匹配返回 ContainerData ──
    // ════════════════════════════════════════════════════════════
    static ContainerData 取容器数据(UIResponder _this, Transform child)
    {
        if (_this.containers == null) return null;
        Transform t = child;
        while (t != null)
        {
            if (t.CompareTag("Container"))
            {
                var rt = t as RectTransform;
                foreach (var cd in _this.containers)
                    if (cd.container == rt) return cd;
                return null;
            }
            t = t.parent;
        }
        return null;
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：清理状态 ──
    // ════════════════════════════════════════════════════════════
    static void 清理(UIResponder _this)
    {
        if (_this.shadowItem != null)
        {
            _this.shadowItem.SetActive(false);
            _this.shadowItem.transform.SetParent(_this.canvas != null ? _this.canvas.transform : _this.transform, false);
        }
        source        = null;
        target        = null;
        itemDragging  = null;
        isLongPress   = false;
        isDrag        = false;
        gridTarget    = null;
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：短按点击 → 显示详情面板 ──
    // ════════════════════════════════════════════════════════════
    static void 显示详情(UIResponder _this, PointerEventData eventData)
    {
        Debug.Log("执行程序：短按普通点击结算 (Short Click)");
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        if (clickedObject == null) return;

        var container = 取容器数据(_this, clickedObject.transform);
        if (container == null || container.items == null) return;

        int index = System.Array.IndexOf(_this.cellRegistry, clickedObject);
        if (index < 0 || index >= container.items.Length) return;

        Item item = container.items[index];
        if (item != null)
        {
            Debug.Log($"点击了索引: {index}, 物品 ID: {item.Id}");
            ShowItemDetail(_this, clickedObject, item.Id.ToString());
        }
        else
        {
            Debug.Log($"点击了索引: {index}, 但该位置没有物品 (空格子)");
        }
    }

    static async void ShowItemDetail(UIResponder _this, GameObject targetObj, string id)
    {
        ItemTable table = await _this.GetItemTable(id);
        if (table != null)
        {
            SetPanelPosition(_this, targetObj);
            SetPanelContent(_this, table);
            if (_this.Panel != null) _this.Panel.gameObject.SetActive(true);
        }
    }

    static void SetPanelContent(UIResponder _this, ItemTable table)
    {
        if (_this.IconImage != null)
        {
            if (_this.IconImage.transform.childCount > 0)
            {
                Transform itemIconTransform = _this.IconImage.transform.GetChild(0);
                Image itemIconImage = itemIconTransform.GetComponent<Image>();
                if (itemIconImage != null)
                {
                    itemIconImage.sprite = table.ItemSprite;
                    itemIconImage.color = Color.white;
                }
                if (itemIconTransform.childCount > 0)
                {
                    Transform glowTransform = itemIconTransform.GetChild(0);
                    Image glowImage = glowTransform.GetComponent<Image>();
                    if (glowImage != null)
                    {
                        glowImage.sprite = table.GlowSprite;
                        glowImage.color = Color.white;
                    }
                }
            }
        }
        if (_this.NameText != null) _this.NameText.text = table.ItemName;
        if (_this.DescText != null) _this.DescText.text = table.ItemDescription;
    }

    static void SetPanelPosition(UIResponder _this, GameObject targetObj)
    {
        if (_this.Panel == null || targetObj == null) return;

        RectTransform targetRT = targetObj.transform as RectTransform;
        RectTransform canvasRT = _this.canvas.transform as RectTransform;

        Vector2 cellCenterLocal = targetRT.rect.center;
        Vector2 canvasCenterPos = UI坐标转换.局部转目标局部(targetRT, canvasRT, cellCenterLocal);
        Vector2 finalPosition;

        if (canvasCenterPos.x >= 0)
        {
            Vector2 cellLeftLocal = new Vector2(targetRT.rect.xMin, targetRT.rect.center.y);
            finalPosition = UI坐标转换.局部转目标局部(targetRT, canvasRT, cellLeftLocal);
            _this.Panel.anchorMin = _this.Panel.anchorMax = new Vector2(0.5f, 0.5f);
            _this.Panel.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            Vector2 cellRightLocal = new Vector2(targetRT.rect.xMax, targetRT.rect.center.y);
            finalPosition = UI坐标转换.局部转目标局部(targetRT, canvasRT, cellRightLocal);
            _this.Panel.anchorMin = _this.Panel.anchorMax = new Vector2(0.5f, 0.5f);
            _this.Panel.pivot = new Vector2(0f, 0.5f);
        }

        float panelHalfHeight = _this.Panel.rect.height / 2f;
        float canvasMaxY = canvasRT.rect.height / 2f;
        float canvasMinY = -canvasMaxY;
        float padding = 2f;

        if (finalPosition.y > 0)
        {
            if (canvasMaxY - finalPosition.y < panelHalfHeight)
                finalPosition.y = canvasMaxY - panelHalfHeight - padding;
        }
        else
        {
            if (finalPosition.y - canvasMinY < panelHalfHeight)
                finalPosition.y = canvasMinY + panelHalfHeight + padding;
        }

        _this.Panel.anchoredPosition = finalPosition;
    }
}
}
