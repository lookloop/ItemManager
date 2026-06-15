using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    Coroutine tipCoroutine;

    public async void FireAndForget(Task task)
    {
        try { await task; }
        catch (Exception e)
        {
            if (tmpTip != null)
            {
                tmpTip.text = $"[Task Error] {e.GetType().Name}: {e.Message}";
                tmpTip.gameObject.SetActive(true);

                if (tipCoroutine != null) StopCoroutine(tipCoroutine);
                tipCoroutine = StartCoroutine(HideTip());
            }
        }
    }

    IEnumerator HideTip()
    {
        yield return new WaitForSeconds(1f);
        if (tmpTip != null)
            tmpTip.gameObject.SetActive(false);
        tipCoroutine = null;
    }
}
}
