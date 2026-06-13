using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class Core
{
    /// <summary>
    /// Core 铺满全屏、沉到最底层、挂透明 Image 接收空白点击。
    /// </summary>
    void SundriesInit()
    {
        //获取canvas，后期使用canvas的摄像头要用
        canvas = GetComponentInParent<Canvas>();

        //获取core层的rect，用于将自己铺满全屏
        var rt = GetComponent<RectTransform>();
        //最小都是0
        rt.anchorMin = Vector2.zero;
        //最大都是1
        rt.anchorMax = Vector2.one;
        //距离最大最小的边距为0，证明已经铺满了
        rt.sizeDelta = Vector2.zero;
        //将自己沉到最底层，避免遮挡其他UI
        rt.SetAsFirstSibling();

        //新建一个image，用于被射线检测
        var img = gameObject.AddComponent<Image>();
        //颜色透明
        img.color = Color.clear;
        //开射线点击
        img.raycastTarget = true;
    }
}
}
