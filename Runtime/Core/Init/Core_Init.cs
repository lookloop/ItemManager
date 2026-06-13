using System.Collections;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    void Awake()
    {

    }

    void Start()
    {
        InitCoreRectAndReceiver();

        BuildAll();
        StartCoroutine(LossTimeLoop());
        BuildDragTool();

#if UNITY_EDITOR
        Test.Fill(this);
#endif
    }

    IEnumerator LossTimeLoop()
    {
        yield return new WaitForSeconds(1800f);
        while (true)
        {
            LossTime();
            yield return new WaitForSeconds(1800f);
        }
    }

}
}
