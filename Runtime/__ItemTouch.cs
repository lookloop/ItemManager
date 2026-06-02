using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Lookloop.ItemManager
{
/// <summary>
/// Item 触控总成 — 黑盒处理所有 Item 交互。
/// UIResponder 只做路由：Item → 调这 3 个方法；Container → 自理。
///
/// ─── 3 个核心字段 ───
///   source       来源 cell
///   target       目标 cell
///   itemDragging 跟随手指的 Item
///
/// ─── 3 个公开方法（距离判定拖拽，不再依赖 BeginDrag）───
///   开始点击  → 记录 source + 起手坐标，启计时器
///   拖拽中    → 距离判定 → isDrag；分流：长按跟随 / 滚Grid
///   结算      → isLongPress + isDrag 组合：交换/复位/详情
/// </summary>
public static class ItemTouch
{
    // ════════════════════════════════════════════════════════════
    // 3 个核心字段
    // ════════════════════════════════════════════════════════════
    public static GameObject source;        // 来源 cell
    public static GameObject target;        // 目标 cell（悬停到的格子）
    public static GameObject itemDragging;  // 跟随手指的漂浮 Item

    // ════════════════════════════════════════════════════════════
    // Grid 运行时引用（由 BackpackBuilder 注入）
    // ════════════════════════════════════════════════════════════
    public static RectTransform gridTransform;
    public static RectTransform maskTransform;
    public static GameObject[]  cellRegistry;
    public static int           cellCount;
    public static int           cellsPerRow;

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
    public static void BeginClick(UIResponder _this, PointerEventData eventData)
    {
        // 重置状态
        isLongPress = false;
        isDrag      = false;
        if (_this.Panel != null) _this.Panel.gameObject.SetActive(false);

        source = eventData.pointerCurrentRaycast.gameObject;
        if (source == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _this.canvas.transform as RectTransform, eventData.position, _this.uiCamera, out beginPosition);

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
        timerCoroutine = _this.StartCoroutine(Timer(_this));
    }

    static IEnumerator Timer(UIResponder _this)
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
    // 2. 拖拽中 — C 阶段（每帧）
    // 距离判定拖拽 → 分流：长按物品跟随 / 短按滚Grid
    // ════════════════════════════════════════════════════════════
    public static void OnDrag(UIResponder _this, PointerEventData eventData)
    {
        // ── 距离判定拖拽 ──
        if (!isDrag)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _this.canvas.transform as RectTransform, eventData.position, _this.uiCamera, out Vector2 currentPos);
            if ((currentPos - beginPosition).sqrMagnitude > 0.01f)
                isDrag = true;
        }

        // ── 分流 ──
        if (isLongPress)
            LongPressFollow(_this, eventData);
        else if (isDrag && gridTarget != null)
            ScrollGrid(_this, eventData);
    }

    // ════════════════════════════════════════════════════════════
    // 3. 结算 — D 阶段（手指抬起）
    // isLongPress + isDrag 组合判定：交换 / 复位 / 详情 / 滚Grid无结算
    // ════════════════════════════════════════════════════════════
    public static void EndDrag(UIResponder _this, PointerEventData eventData)
    {
        // 先停计时器
        if (timerCoroutine != null)
        {
            _this.StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        if (isLongPress)
        {
            if (isDrag)  Swap(_this);
            else         ResetPosition(_this);
        }
        else if (!isDrag)
        {
            ShowDetail(_this, eventData);
        }
        // else: 短按拖拽 — 滚 Grid 最后一帧已到位，无结算

        Cleanup(_this);
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：长按物品跟随手指 + 阴影悬停检测 ──
    // ════════════════════════════════════════════════════════════
    static void LongPressFollow(UIResponder _this, PointerEventData eventData)
    {
        Debug.Log("UI 长按拖拽: " + _this.gameObject.name);

        // 物品跟随
        if (itemDragging != null)
        {
            RectTransform rt = itemDragging.transform as RectTransform;
            if (rt != null)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _this.canvas.transform as RectTransform, eventData.position, _this.uiCamera, out pos);
                rt.anchoredPosition = pos;
            }
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
    static void ScrollGrid(UIResponder _this, PointerEventData eventData)
    {
        RectTransform maskRT = gridTarget.parent as RectTransform;
        if (maskRT == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _this.canvas.transform as RectTransform, eventData.position, _this.uiCamera, out Vector2 now);
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
    static void ResetPosition(UIResponder _this)
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
    static void Swap(UIResponder _this)
    {
        if (!TrySwap(_this))
            ResetPosition(_this);
    }

    static bool TrySwap(UIResponder _this)
    {
        if (target == null || target == source) return false;

        var cont = GetContainerData(_this, source.transform);
        if (cont == null || cont.items == null) return false;

        int srcIdx = System.Array.IndexOf(ItemTouch.cellRegistry, source);
        int dstIdx = System.Array.IndexOf(ItemTouch.cellRegistry, target);
        if (srcIdx < 0 || dstIdx < 0 || srcIdx >= cont.items.Length || dstIdx >= cont.items.Length)
            return false;

        var srcItem = cont.items[srcIdx];
        var dstItem = cont.items[dstIdx];
        ItemDataManager.SetCell(_this, srcIdx, dstItem);
        ItemDataManager.SetCell(_this, dstIdx, srcItem);

        if (itemDragging != null) Object.Destroy(itemDragging);

        Debug.Log($"交换: {srcIdx} ↔ {dstIdx}");
        return true;
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：从子级往上找 Container，匹配返回 ContainerData ──
    // ════════════════════════════════════════════════════════════
    static ContainerData GetContainerData(UIResponder _this, Transform child)
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
    static void Cleanup(UIResponder _this)
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
    static void ShowDetail(UIResponder _this, PointerEventData eventData)
    {
        Debug.Log("执行程序：短按普通点击结算 (Short Click)");
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        if (clickedObject == null) return;

        var container = GetContainerData(_this, clickedObject.transform);
        if (container == null || container.items == null) return;

        int index = System.Array.IndexOf(ItemTouch.cellRegistry, clickedObject);
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

        var panelRT  = _this.Panel;
        var canvasRT = _this.canvas.transform as RectTransform;

        // 挂到 cell 下，锚点居中，归零 → Panel 中心对齐 cell 中心
        panelRT.SetParent(targetObj.transform, false);
        panelRT.anchorMin = panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.anchoredPosition = Vector2.zero;

        // 根据 cell 在屏幕左右 → pivot 控制 Panel 向左/右伸出
        bool onRight = targetObj.transform.position.x > Screen.width * 0.5f;
        panelRT.pivot = onRight ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f);

        // 移回 Canvas（避免 Mask 裁切），保持世界位置
        panelRT.SetParent(canvasRT, true);

        // Y 轴越界钳位
        float halfH = panelRT.rect.height * 0.5f;
        float limit = canvasRT.rect.height * 0.5f;
        float y = panelRT.anchoredPosition.y;
        if      (y + halfH >  limit) y =  limit - halfH - 2f;
        else if (y - halfH < -limit) y = -limit + halfH + 2f;
        panelRT.anchoredPosition = new Vector2(panelRT.anchoredPosition.x, y);
    }
}
}
