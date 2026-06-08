using System.Collections;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        ContainerBuilder.BuildAll(this);
        StartCoroutine(LossTimeLoop());

    }
    IEnumerator LossTimeLoop()
{
    yield return new WaitForSeconds(1800f);  // 首次等 30 分钟
    while (true)
    {
        LossTime();
        yield return new WaitForSeconds(1800f);  // 每 30 分钟一次
    }
}

}
}