using System.Collections;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    Coroutine tipCoroutine;

    /// <summary>
    /// 在屏幕中央显示白色临时提示，duration 秒后自动消失。新提示重置计时。
    /// </summary>
    public void ShowTip(string text, float duration = 1f)
    {
        ShowTip(text, duration, Color.white);
    }

    /// <summary>
    /// 指定颜色的临时提示。
    /// </summary>
    public void ShowTip(string text, float duration, Color color)
    {
        if (tmpTip == null) return;

        tmpTip.text = text;
        tmpTip.color = color;
        tmpTip.gameObject.SetActive(true);

        if (tipCoroutine != null)
            StopCoroutine(tipCoroutine);
        tipCoroutine = StartCoroutine(HideTipAfter(duration));
    }

    IEnumerator HideTipAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (tmpTip != null)
            tmpTip.gameObject.SetActive(false);
        tipCoroutine = null;
    }
}
}
