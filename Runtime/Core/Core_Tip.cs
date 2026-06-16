using System.Collections;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    Coroutine tipCoroutine;

    /// <summary>
    /// 在屏幕中央显示临时提示，duration 秒后自动消失。
    /// 新的提示会重置计时。
    /// </summary>
    public void ShowTip(string text, float duration = 1f, Color? color = null)
    {
        if (tmpTip == null) return;

        tmpTip.text = text;
        tmpTip.color = color ?? Color.white;
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
