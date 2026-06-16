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
        InitRect();
        BuildAll();
        StartCoroutine(LossTimeLoop());

        // Debug: fill containers with random items for manual testing
        Test.Fill(this);
    }

    

}
}
