using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class Core
{
    /// <summary>
    /// Core 铺满全屏、沉到最底层、挂透明 Image 接收空白点击。
    /// </summary>
    void InitCoreRectAndReceiver()
    {
        canvas = GetComponentInParent<Canvas>();


        
        var rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.SetAsFirstSibling();

        var img = gameObject.AddComponent<Image>();
        img.color = Color.clear;
        img.raycastTarget = true;
    }
}
}
