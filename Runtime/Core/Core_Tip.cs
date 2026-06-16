using System.Collections;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    Coroutine tipCoroutine;

    /// <summary>
    /// Shows a white temporary message at the center of the screen that
    /// auto-hides after <paramref name="duration"/> seconds. Calling again
    /// resets the timer.
    /// </summary>
    public void ShowTip(string text, float duration = 1f)
    {
        ShowTip(text, duration, Color.white);
    }

    /// <summary>
    /// Same as <c>ShowTip(text, duration)</c> but lets you pick the color.
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
