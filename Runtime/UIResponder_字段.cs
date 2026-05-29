using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class UIResponder
{
    public enum Mode { Scroll, Page, Fixed }

    [Header("容器模式")]
    public Mode mode = Mode.Scroll;

    [Header("Grid 参数")]
    public int rows = 5;
    public int cols = 4;
    public int totalItems = 20;
    [Tooltip("格子边长 (mm)")]
    public float cellWidth = 10f;
    [Tooltip("物品图标边长 (mm)")]
    public float itemWidth = 8f;
    public float cellSpacing = 0f;

    [Header("计时器")]
    public float timerValue = 0.3f;
    public float dragDeadzone = 5f;

    [Header("视觉")]
    public Sprite backpackSprite;
    public Sprite maskSprite;
    public Sprite cellSprite;
    public TMPro.TMP_FontAsset itemFont;

    [Header("Mask")]
    public float maskHeight = 40f;
    public float maskPosY = -8f;

    [Header("面板")]
    public float horizontalPadding = 2f;
    public float backpackExtraHeight = 12f;
    public bool autoBuild = true;

    [Header("拖拽视觉")]
    public GameObject shadowItem;

    [Header("详情面板")]
    public RectTransform Panel;
    public TMPro.TextMeshProUGUI NameText;
    public TMPro.TextMeshProUGUI DescText;
    public Image IconImage;

    [Header("3D换装联动")]
    public long accountId = 19194472025L;
    public RectTransform[] equipmentSlots;

    // ════════════════════════════════════════
    // 运行时字段 — Inspector 不可见
    // ════════════════════════════════════════

    [HideInInspector] public Canvas canvas;
    [HideInInspector] public bool isLongPress;
    [HideInInspector] public bool isDrag;

    [HideInInspector] public Vector2 beginPosition;
    [HideInInspector] public Vector2 endPosition;
    [HideInInspector] public Vector2 gridPosition;
    [HideInInspector] public Vector2 backpackPosition;

    [HideInInspector] public RectTransform gridTransform;
    [HideInInspector] public RectTransform maskTransform;
    [HideInInspector] public RectTransform backpackPanel;

    [HideInInspector] public int cellCount;
    [HideInInspector] public int cellsPerRow;

    [HideInInspector] public GameObject[] cellRegistry;
    [HideInInspector] public Item[] items;
    [HideInInspector] public int currentPage;

    [HideInInspector] public GameObject sourceItem;
    [HideInInspector] public GameObject sourceObject;
    [HideInInspector] public GameObject targetObject;

    [HideInInspector] public Coroutine timerCoroutine;
    [HideInInspector] public Item[] equippedItems;

    // 静态映射
    public static readonly int[] TypeToEquipIndex = { 0, 2, 1, 3, 4 };
    public static readonly int[] TypeTo3DKey = { 0, 1001, 1002, 1003, 1004 };
}
}
