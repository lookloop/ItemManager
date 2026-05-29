using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
public static class UI坐标转换
{
    /// <summary>
    /// 根据屏幕像素坐标，获取该点在目标对象上的局部 xy 坐标 (Screen -> Local)
    /// 内部方法，不对外开放。
    /// </summary>
    private static Vector2 屏幕坐标转局部坐标(RectTransform rectTransform, Vector2 screenPoint, Camera camera)
    {
        if (rectTransform == null) return Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, camera, out Vector2 localPoint);
        return localPoint;
    }

    /// <summary>
    /// 自动获取 UI 对象所在 Canvas 的正确摄像机
    /// </summary>
    public static Camera 获取Canvas摄像机(RectTransform rt)
    {
        if (rt == null) return null;
        Canvas canvas = rt.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            return canvas.worldCamera;
        }
        return null;
    }

    /// <summary>
    /// 将源对象的局部坐标转换为目标对象的局部坐标
    /// </summary>
    public static Vector2 局部转目标局部(RectTransform sourceRT, RectTransform targetRT, Vector2 sourceLocalPoint)
    {
        if (sourceRT == null || targetRT == null) return Vector2.zero;

        // 1. 获取源对象的摄像机，将源局部坐标转换为世界坐标，再转换为屏幕坐标（像素化）
        Camera sourceCamera = 获取Canvas摄像机(sourceRT);
        Vector3 worldPoint = sourceRT.TransformPoint(sourceLocalPoint);
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(sourceCamera, worldPoint);

        // 2. 获取目标对象的摄像机，将屏幕坐标转换为目标对象的局部坐标
        Camera targetCamera = 获取Canvas摄像机(targetRT);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRT, screenPoint, targetCamera, out Vector2 targetLocalPoint);

        return targetLocalPoint;
    }

    /// <summary>
    /// 快捷方法：将当前事件直接转换为目标对象的局部坐标
    /// </summary>
    /// <param name="target">目标 UI 对象</param>
    /// <param name="eventData">UI 事件数据</param>
    public static Vector2 获取事件局部坐标(RectTransform target, PointerEventData eventData)
    {
        if (target == null || eventData == null) return Vector2.zero;
        return 屏幕坐标转局部坐标(target, eventData.position, eventData.pressEventCamera);
    }
}
}