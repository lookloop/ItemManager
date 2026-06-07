using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public static class CellTouch_Drag
{
    static float edgeTimer;
    static int edgeZone;
    const float EdgeHoldTime = 1f;
    public static PointerEventData lastEventData;

    /// <summary>拖拽事件 — 跟手 + 存事件引用</summary>
    public static void OnDrag(Core core, PointerEventData eventData)
    {
        lastEventData = eventData;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            core.canvas.transform as RectTransform,
            eventData.position,
            core.canvas.worldCamera,
            out Vector2 localPos);
        MiscInit.parent.anchoredPosition = localPos;
    }

    /// <summary>每帧调用 — 边缘检测 + 滚动/翻页</summary>
    public static void Update(Core core)
    {
        if (lastEventData == null) return;

        var hitGo = lastEventData.pointerCurrentRaycast.gameObject;
        if (hitGo == null) return;

        // 从命中点向上追溯到所属 ContainerMod（不要求 Cell，任何容器内元素即可）
        ContainerMod targetMod = null;
        foreach (var m in ContainerManager.containers)
        {
            if (hitGo.transform.IsChildOf(m.container))
            {
                targetMod = m;
                break;
            }
        }
        if (targetMod == null) return;

        var mask = targetMod.mask;
        var grid = targetMod.grid;
        if (mask == null || grid == null) return;

        // 坐标 → mask 本地
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mask, lastEventData.position, core.canvas.worldCamera, out Vector2 maskLocal);

        var r = mask.rect;
        float edge = r.width * 0.15f;

        bool nearLeft   = maskLocal.x < r.xMin + edge;
        bool nearRight  = maskLocal.x > r.xMax - edge;
        bool nearTop    = maskLocal.y > r.yMax - edge;
        bool nearBottom = maskLocal.y < r.yMin + edge;

        // 上下 — 匀速滚动
        float scrollSpeed = 120f * Time.deltaTime;
        float maxScroll = grid.sizeDelta.y - mask.sizeDelta.y;

        if (nearTop)
            grid.anchoredPosition = new Vector2(0, Mathf.Clamp(grid.anchoredPosition.y - scrollSpeed, 0f, maxScroll));
        else if (nearBottom)
            grid.anchoredPosition = new Vector2(0, Mathf.Clamp(grid.anchoredPosition.y + scrollSpeed, 0f, maxScroll));

        // 左右 — 计时翻页
        int currentZone = 0;
        if (nearLeft)  currentZone = 1;
        if (nearRight) currentZone = 2;

        if (currentZone == 0)
        {
            edgeTimer = 0f;
        }
        else
        {
            if (currentZone != edgeZone)
            {
                edgeTimer = 0f;
                edgeZone = currentZone;
            }
            edgeTimer += Time.deltaTime;

            if (edgeTimer >= EdgeHoldTime)
            {
                edgeTimer = 0f;
                if (currentZone == 1)
                    TurnPageTouch.PrevPage(core, targetMod);
                else
                    TurnPageTouch.NextPage(core, targetMod);
            }
        }
    }
}

}
