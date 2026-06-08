using System.Collections;
using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    void Awake()
    {
        //frame设置为60
        Application.targetFrameRate = 60;
        //获取一下canvas是ui工作当中很重要的事情
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        //启动构建功工作
        ContainerBuilder.BuildAll(this);
        //开启磁盘资源本地缓存循环检查，去除时间留得太久的
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