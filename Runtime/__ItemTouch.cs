using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Lookloop.ItemManager
{
/// <summary>
/// Item 触控总成 — 黑盒处理所有 Item 交互。
/// Core 只做路由：Item → 调这 3 个方法；Container → 自理。
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
    // Grid 运行时引用（由 ContainerBuilder 注入）
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

    // 快捷引用
    static ContainerSpec B => ContainerManager.containers != null && ContainerManager.containers.Count > 0
        ? ContainerManager.containers[0].blueprint : null;

    // ════════════════════════════════════════════════════════════
    // 1. 开始点击 — A 阶段
    // ════════════════════════════════════════════════════════════
    public static void BeginClick(Core core, PointerEventData eventData)
    {
        // 重置状态
        isLongPress = false;
        isDrag      = false;

        // 隐藏所有容器的详情面板
        HideAllDetailPanels();

        source = eventData.pointerCurrentRaycast.gameObject;
        if (source == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            core.canvas.transform as RectTransform, eventData.position, core.canvas?.worldCamera, out beginPosition);

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
            core.StopCoroutine(timerCoroutine);
        timerCoroutine = core.StartCoroutine(Timer(core));
    }

    static IEnumerator Timer(Core core)
    {
        float tv = B != null ? B.timerValue : 0.3f;
        yield return new WaitForSeconds(tv);

        // ── 长按成功：拾起物品 ──
        isLongPress = true;

        if (source != null && source.transform.childCount > 0)
        {
            itemDragging = source.transform.GetChild(0).gameObject;
            itemDragging.transform.SetParent(core.canvas.transform, false);
            float cw = B != null ? B.cellWidth : 10f;
            itemDragging.transform.localScale = new Vector3(cw / 10f, cw / 10f, 1f);
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
    public static void OnDrag(Core core, PointerEventData eventData)
    {
        // ── 距离判定拖拽 ──
        if (!isDrag)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                core.canvas.transform as RectTransform, eventData.position, core.canvas?.worldCamera, out Vector2 currentPos);
            if ((currentPos - beginPosition).sqrMagnitude > 0.01f)
                isDrag = true;
        }

        // ── 分流 ──
        if (isLongPress)
            LongPressFollow(core, eventData);
        else if (isDrag && gridTarget != null)
            ScrollGrid(core, eventData);
    }

    // ════════════════════════════════════════════════════════════
    // 3. 结算 — D 阶段（手指抬起）
    // isLongPress + isDrag 组合判定：交换 / 复位 / 详情 / 滚Grid无结算
    // ════════════════════════════════════════════════════════════
    public static void EndDrag(Core core, PointerEventData eventData)
    {
        // 先停计时器
        if (timerCoroutine != null)
        {
            core.StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        if (isLongPress)
        {
            if (isDrag)  Swap(core);
            else         ResetPosition(core);
        }
        else if (!isDrag)
        {
            ShowDetail(core, eventData);
        }
        // else: 短按拖拽 — 滚 Grid 最后一帧已到位，无结算

        Cleanup(core);
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：长按物品跟随手指 + 阴影悬停检测 ──
    // ════════════════════════════════════════════════════════════
    static void LongPressFollow(Core core, PointerEventData eventData)
    {
        Debug.Log("UI 长按拖拽: " + core.gameObject.name);

        // 物品跟随
        if (itemDragging != null)
        {
            RectTransform rt = itemDragging.transform as RectTransform;
            if (rt != null)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    core.canvas.transform as RectTransform, eventData.position, core.canvas?.worldCamera, out pos);
                rt.anchoredPosition = pos;
            }
        }

        // 射线检测悬停格子
        var shadow = B != null ? B.shadowItem : null;
        GameObject hoverObj = eventData.pointerCurrentRaycast.gameObject;
        if (hoverObj != null && hoverObj.CompareTag("Item"))
        {
            if (target != hoverObj)
            {
                target = hoverObj;
                if (shadow != null)
                {
                    shadow.SetActive(true);
                    shadow.transform.SetParent(hoverObj.transform, false);
                    (shadow.transform as RectTransform).anchoredPosition = Vector2.zero;
                    shadow.transform.SetAsLastSibling();
                }
            }
        }
        else
        {
            if (target != null)
            {
                target = null;
                if (shadow != null)
                {
                    shadow.SetActive(false);
                    shadow.transform.SetParent(core.canvas.transform, false);
                }
            }
        }
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：短按滚 Grid（X 锁定，Y 钳位）──
    // ════════════════════════════════════════════════════════════
    static void ScrollGrid(Core core, PointerEventData eventData)
    {
        RectTransform maskRT = gridTarget.parent as RectTransform;
        if (maskRT == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            core.canvas.transform as RectTransform, eventData.position, core.canvas?.worldCamera, out Vector2 now);
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
    static void ResetPosition(Core core)
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
    static void Swap(Core core)
    {
        if (!TrySwap(core))
            ResetPosition(core);
    }

    static bool TrySwap(Core core)
    {
        if (target == null || target == source) return false;

        var cont = GetContainer(core, source.transform);
        if (cont == null || cont.items == null) return false;

        int srcIdx = System.Array.IndexOf(cellRegistry, source);
        int dstIdx = System.Array.IndexOf(cellRegistry, target);
        if (srcIdx < 0 || dstIdx < 0 || srcIdx >= cont.items.Length || dstIdx >= cont.items.Length)
            return false;

        var srcItem = cont.items[srcIdx];
        var dstItem = cont.items[dstIdx];
        ItemDataManager.SetCell(core, srcIdx, dstItem);
        ItemDataManager.SetCell(core, dstIdx, srcItem);

        if (itemDragging != null) Object.Destroy(itemDragging);

        Debug.Log($"交换: {srcIdx} ↔ {dstIdx}");
        return true;
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：从子级往上找 Container，匹配返回 ContainerMod ──
    // ════════════════════════════════════════════════════════════
    static ContainerMod GetContainer(Core core, Transform child)
    {
        if (ContainerManager.containers == null) return null;
        Transform t = child;
        while (t != null)
        {
            if (t.CompareTag("Container"))
            {
                var rt = t as RectTransform;
                foreach (var item in ContainerManager.containers)
                    if (item.container == rt) return item;
                return null;
            }
            t = t.parent;
        }
        return null;
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：清理状态 ──
    // ════════════════════════════════════════════════════════════
    static void Cleanup(Core core)
    {
        var shadow = B != null ? B.shadowItem : null;
        if (shadow != null)
        {
            shadow.SetActive(false);
            shadow.transform.SetParent(core.canvas != null ? core.canvas.transform : core.transform, false);
        }
        source        = null;
        target        = null;
        itemDragging  = null;
        isLongPress   = false;
        isDrag        = false;
        gridTarget    = null;
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：隐藏所有容器的详情面板 ──
    // ════════════════════════════════════════════════════════════
    static void HideAllDetailPanels()
    {
        if (ContainerManager.containers == null) return;
        foreach (var cd in ContainerManager.containers)
        {
            if (cd.activeDetailPanel != null)
            {
                Object.Destroy(cd.activeDetailPanel);
                cd.activeDetailPanel = null;
            }
            if (cd.blueprint != null && cd.blueprint.detailPanel != null)
                cd.blueprint.detailPanel.gameObject.SetActive(false);
        }
    }

    // ════════════════════════════════════════════════════════════
    // ── 内部：短按点击 → 显示详情面板 ──
    // ════════════════════════════════════════════════════════════
    static void ShowDetail(Core core, PointerEventData eventData)
    {
        Debug.Log("执行程序：短按普通点击结算 (Short Click)");
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        if (clickedObject == null) return;

        var container = GetContainer(core, clickedObject.transform);
        if (container == null || container.items == null) return;

        int index = System.Array.IndexOf(cellRegistry, clickedObject);
        if (index < 0 || index >= container.items.Length) return;

        Item item = container.items[index];
        if (item != null)
        {
            Debug.Log($"点击了索引: {index}, 物品 ID: {item.Id}");
            ShowItemDetail(core, container, clickedObject, item.Id.ToString());
        }
        else
        {
            Debug.Log($"点击了索引: {index}, 但该位置没有物品 (空格子)");
        }
    }

    static async void ShowItemDetail(Core core, ContainerMod container, GameObject targetObj, string id)
    {
        var bp = container.blueprint;
        if (bp == null) return;

        ItemTable table = await core.GetItemTable(id);
        if (table == null) return;

        // 销毁旧面板（同一容器）
        if (container.activeDetailPanel != null)
        {
            Object.Destroy(container.activeDetailPanel);
            container.activeDetailPanel = null;
        }

        RectTransform panelRT;

        if (bp.detailPanelPrefab != null)
        {
            // ── Prefab 模式：Instantiate ──
            var instance = Object.Instantiate(bp.detailPanelPrefab, core.canvas.transform);
            container.activeDetailPanel = instance;
            panelRT = instance.transform as RectTransform;
            if (panelRT == null) panelRT = instance.AddComponent<RectTransform>();
        }
        else if (bp.detailPanel != null)
        {
            // ── 场景引用模式 ──
            panelRT = bp.detailPanel;
        }
        else
        {
            return; // 无可用面板
        }

        SetPanelContent(bp, table);
        SetPanelPosition(core, panelRT, targetObj);
        panelRT.gameObject.SetActive(true);
    }

    static void SetPanelContent(ContainerSpec bp, ItemTable table)
    {
        if (bp.iconImage != null)
        {
            if (bp.iconImage.transform.childCount > 0)
            {
                Transform itemIconTransform = bp.iconImage.transform.GetChild(0);
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
        if (bp.nameText != null) bp.nameText.text = table.ItemName;
        if (bp.descText != null) bp.descText.text = table.ItemDescription;
    }

    static void SetPanelPosition(Core core, RectTransform panelRT, GameObject targetObj)
    {
        if (panelRT == null || targetObj == null) return;

        var canvasRT = core.canvas.transform as RectTransform;

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