using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class UIResponder
{
    public enum Mode { Scroll, Page, Fixed }

    // ════════════════════════════════════════
    // Inspector 字段
    // ════════════════════════════════════════

    [Header("容器模式")]
    public Mode mode = Mode.Scroll;

    [Header("Grid 参数")]
    public int rows = 5;                                  // → UIResponder.Start
    public int cols = 4;                                  // → UIResponder.Start
    public int totalItems = 20;                           // → UIResponder.BuildData
    [Tooltip("格子边长 (mm)")]
    public float cellWidth = 10f;                         // → __背包生成器.Build  __背包初始化.ApplyCellPositions  B_长按开始.Execute
    [Tooltip("物品图标边长 (mm)")]
    public float itemWidth = 8f;                          // → __背包初始化.创建Item
    public float cellSpacing = 0f;                        // 预留

    [Header("计时器")]
    public float timerValue = 0.3f;                       // → A_开始.计时器
    public float dragDeadzone = 5f;                       // → UIResponder.OnBeginDrag

    [Header("视觉")]
    public Sprite backpackSprite;                         // → __背包生成器.Build
    public Sprite maskSprite;                             // → __背包生成器.Build
    public Sprite cellSprite;                             // → __背包生成器.Build
    public TMPro.TMP_FontAsset itemFont;                  // → __背包初始化.创建Item

    [Header("Mask")]
    public float maskHeight = 40f;                        // → __背包初始化.ApplyGridSize  C_短按拖拽中  D_短按拖拽结束
    public float maskPosY = -8f;                          // → __背包初始化.ApplyGridSize

    [Header("面板")]
    public float horizontalPadding = 2f;                  // → __背包初始化.ApplyGridSize
    public float backpackExtraHeight = 12f;               // → __背包初始化.ApplyGridSize
    public bool autoBuild = true;                         // → UIResponder.Start

    [Header("拖拽视觉")]
    public GameObject shadowItem;                         // → C_长按拖拽中  UIResponder.ClearDragState

    [Header("详情面板")]
    public RectTransform Panel;                           // → A_开始  D_短按点击结束.SetPanelPosition
    public TMPro.TextMeshProUGUI NameText;                // → D_短按点击结束.SetPanelContent
    public TMPro.TextMeshProUGUI DescText;                // → D_短按点击结束.SetPanelContent
    public Image IconImage;                               // → D_短按点击结束.SetPanelContent

    [Header("3D换装联动")]
    public long accountId = 19194472025L;                 // 预留
    public RectTransform[] equipmentSlots;                // → D_长按拖拽结束.GetEquipSlotIndex

    // ════════════════════════════════════════
    // 运行时字段 — Inspector 不可见
    // ════════════════════════════════════════

    [HideInInspector] public Canvas canvas;               // → Awake  A_开始  B_长按开始  C_短按拖拽中  C_长按拖拽中  D_短按点击结束  ClearDragState
    [HideInInspector] public bool isLongPress;            // → B_长按开始(写)  UIResponder.OnDrag/OnPointerUp(读)
    [HideInInspector] public bool isDrag;                 // → UIResponder.OnBeginDrag(写)  OnPointerUp(读/写)

    [HideInInspector] public Vector2 beginPosition;       // → A_开始(写)  B_长按开始  C_短按拖拽中(读)
    [HideInInspector] public Vector2 endPosition;         // → C_短按拖拽中(写)
    [HideInInspector] public Vector2 gridPosition;        // → A_开始(写)  C_短按拖拽中(读)
    [HideInInspector] public Vector2 backpackPosition;    // → A_开始(写)  C_短按拖拽中.拖拽背包面板(读)

    [HideInInspector] public RectTransform gridTransform; // → __背包生成器(写)  __背包初始化.ApplyGridSize  A_开始  C_短按拖拽中  D_短按拖拽结束
    [HideInInspector] public RectTransform maskTransform; // → __背包生成器(写)  __背包初始化.ApplyGridSize  C_短按拖拽中  D_短按拖拽结束
    [HideInInspector] public RectTransform backpackPanel; // → __背包生成器(写)  __背包初始化.ApplyGridSize

    [HideInInspector] public int cellCount;               // → Start(写)  __背包生成器  __背包初始化  SyncPage/BuildData/SetCell
    [HideInInspector] public int cellsPerRow;             // → Start(写)  __背包初始化.ApplyCellPositions/ApplyGridSize

    [HideInInspector] public GameObject[] cellRegistry;   // → __背包生成器(写)  __背包初始化  D_短按点击结束  D_长按拖拽结束
    [HideInInspector] public Item[] items;                // → BuildData/SyncPage/SetCell  D_短按点击结束  D_长按拖拽结束  __背包初始化.设置格子
    [HideInInspector] public int currentPage;             // → SetCell/NextPage/PrevPage/SyncPage

    [HideInInspector] public GameObject sourceItem;       // → B_长按开始(写)  C_长按拖拽中  D_长按拖拽结束  D_长按点击结束  ClearDragState(清)
    [HideInInspector] public GameObject sourceObject;     // → A_开始(写)  B_长按开始  C_短按拖拽中  D_短按拖拽结束  D_长按拖拽结束  D_长按点击结束  ClearDragState(清)
    [HideInInspector] public GameObject targetObject;     // → C_长按拖拽中(写)  D_长按拖拽结束(读)  ClearDragState(清)

    [HideInInspector] public Coroutine timerCoroutine;    // → A_开始(写)  UIResponder.OnBeginDrag/OnPointerUp(停)
    [HideInInspector] public Item[] equippedItems;        // → Start(初始化)  D_长按拖拽结束(读写)

    // 静态映射
    public static readonly int[] TypeToEquipIndex = { 0, 2, 1, 3, 4 };   // → D_长按拖拽结束(已移除3D回调, 预留)
    public static readonly int[] TypeTo3DKey    = { 0, 1001, 1002, 1003, 1004 }; // → 预留
}
}
