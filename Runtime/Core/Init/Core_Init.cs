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
        SundriesInit();

        BuildAll();
        StartCoroutine(LossTimeLoop());
        BuildDragTool();

#if UNITY_EDITOR
        Test.Fill(this);
#endif
    }

    

}
}
