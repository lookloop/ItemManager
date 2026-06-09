using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class Core
{
    //承担一切的初始化工作
    void Awake()
    {
        //frame设置为60
        Application.targetFrameRate = 60;
        //获取一下canvas是ui工作当中很重要的事情
        canvas = GetComponentInParent<Canvas>();

        // 全屏透明接收器 — 确保点击空白也能被 Core 捕获
        var rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.SetAsLastSibling();  // 最高级，背包打开时接管所有点击

        var img = gameObject.AddComponent<Image>();
        img.color = Color.clear;
        img.raycastTarget = true;
    }

    void Start()
    {
        //启动构建工作
        ContainerBuilder.BuildAll(this);
        //开启磁盘资源本地缓存循环检查，去除时间留得太久的
        StartCoroutine(LossTimeLoop());

        //构建拖拽工具
        OtherTool.BuildDragItem(this);
        OtherTool.BuildShadow(this);

        Test.Fill(this);
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