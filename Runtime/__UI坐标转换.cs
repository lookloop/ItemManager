using UnityEngine;

namespace Lookloop.ItemManager
{
public static class UI坐标转换
{
    public static Camera camera;

    /// <summary>局部坐标 → 屏幕像素</summary>
    public static Vector2 局部转像素(RectTransform rt, Vector2 localPoint)
    {
        Vector3 world = rt.TransformPoint(localPoint);
        return RectTransformUtility.WorldToScreenPoint(camera, world);
    }

    /// <summary>屏幕像素 → 局部坐标</summary>
    public static Vector2 像素转局部(RectTransform rt, Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPoint, camera, out Vector2 local);
        return local;
    }
}
}
