using UnityEngine;

namespace Lookloop.ItemManager
{
public partial class Core
{
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        ContainerBuilder.BuildAll(this);
    }

}
}