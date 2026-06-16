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
        //启用杂项
        SundriesInit();
        //启用拖拽时专用工具构建
        InitRect();
        //启用初始构建
        BuildAll();
        //启用addres缓存计时器
        StartCoroutine(LossTimeLoop());
        


//临时放点东西，提供我手动测试
        Test.Fill(this);

    }

    

}
}
