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
        TestItemFiller.FillAll(this, 20);
        InvokeRepeating(nameof(ReleaseStaleHandles), 1800f, 1800f); // 每 30 分钟清理过期句柄
    }

}
}