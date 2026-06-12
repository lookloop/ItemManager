using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 共享工具 — CreateRect 供 ContainerBuilder 和 DragTool 共用。
    /// </summary>
    public static class RectUtility
    {
        public static RectTransform CreateRect(string name, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPosition, Vector2 sizeDelta,
            string tag = null,
            params System.Type[] components)
        {
            var types = new System.Type[components.Length + 1];
            types[0] = typeof(RectTransform);
            for (int i = 0; i < components.Length; i++)
                types[i + 1] = components[i];

            var go = new GameObject(name, types);
            var rect = go.transform as RectTransform;
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            if (tag != null) go.tag = tag;
            return rect;
        }
    }
}
