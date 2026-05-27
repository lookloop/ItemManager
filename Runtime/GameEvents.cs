using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 跨容器 static event 总线 — 所有包通过这里通信，互不引用。
/// </summary>
public static class GameEvents
{
    /// <summary>Cell A 和 Cell B 交换 Key（跨容器拖拽松手）</summary>
    public static System.Action<CellContainer, int, CellContainer, int> OnCellSwap;

    /// <summary>Item 从 from 容器移动到 to 容器</summary>
    public static System.Action<CellContainer, int, CellContainer, int> OnItemMove;

    /// <summary>某个 Cell 内容被修改（通知所有容器刷新）</summary>
    public static System.Action<CellContainer, int> OnCellModified;

    /// <summary>装备请求: (accountId, equipTypeIndex, key3D)</summary>
    public static System.Action<long, int, int> OnEquipRequest;

    /// <summary>炼丹/锻造: 消耗输入Cell → 生成输出Item</summary>
    public static System.Action<int[], int, int> OnCraftRequest;
}