using UnityEngine;

public partial class UIResponder
{
    void Start()
    {
        // ── 自动构建背包层级 ──
        if (autoBuild && gridTransform == null)
            GridGenerator.Build(this);

        // ── 委托挂载 ──
        注册委托列表();

        // 初始化装备槽数据
        if (equippedItems == null || equippedItems.Length != 4)
            equippedItems = new Item[4];

        背包初始化.Execute(this);
    }
}
