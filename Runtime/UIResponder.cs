using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class UIResponder : MonoBehaviour, 
    IPointerDownHandler, 
    IPointerUpHandler, 
    IBeginDragHandler, 
    IDragHandler
{
    public Canvas canvas; // 当前 UI 所在的 Canvas，用于坐标转换参考

    void Awake()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    [Header("计时器")]
    public float timerValue = 0.3f; // 长按判定的时间阈值（秒）
    public float dragDeadzone = 5f;  // 拖拽死区（像素），小于此值不触发 OnBeginDrag
    [HideInInspector] public Coroutine timerCoroutine; // 长按计时器协程

    [Header("状态")]
    [HideInInspector] public bool isLongPress = false;
    [HideInInspector] public bool isDrag = false;
    [HideInInspector] public int onlyResponder = -1;

    [Header("交互数据记录")]
    [HideInInspector] public Vector2 beginPosition;
    [HideInInspector] public Vector2 endPosition;

    [Header("背包自动构建")]
    public bool autoBuild = true;
    public Sprite backpackSprite;
    public Sprite maskSprite;
    public Sprite cellSprite;
    public TMPro.TMP_FontAsset itemFont;

    [Header("滑动背包功能")]
    [HideInInspector] public Vector2 gridPosition;
    [HideInInspector] public Vector2 backpackPosition;
    [HideInInspector] public RectTransform gridTransform;
    [HideInInspector] public RectTransform maskTransform;
    [HideInInspector] public RectTransform backpackPanel;
    //背包格子相关生成
    [Header("背包格子相关生成")]
    public int cellCount = 20;
    public int cellsPerRow = 5;
    [Tooltip("格子边长 (mm)")]
    public float cellWidth = 10f;
    [Tooltip("物品图标边长 (mm)")]
    public float itemWidth = 8f;
    [Tooltip("X轴两侧扩张 (mm)，用于背包面板外框")]
    public float horizontalPadding = 0f;
    [Tooltip("Mask 可视高度 (mm)")]
    public float maskHeight = 200f;
    [Tooltip("背包面板额外高度 (mm)，加在 mask 之上")]
    public float backpackExtraHeight = 40f;
    [Tooltip("Mask Y 位置 (mm)，正=下 负=上")]
    public float maskPosY = -20f;
    [HideInInspector] public GameObject[] cellRegistry;
    [HideInInspector] public Item[] items;


    [Header("拖拽视觉表现")]
    [HideInInspector] public GameObject sourceItem;
    [HideInInspector] public GameObject sourceObject;
    [HideInInspector] public GameObject targetObject;
    public GameObject shadowItem;

    [Header("详情面板")]
    public RectTransform Panel; // 详情面板的 RectTransform 容器
    public TMPro.TextMeshProUGUI NameText; // 详情面板：显示物品名称的文本组件
    public TMPro.TextMeshProUGUI DescText; // 详情面板：显示物品描述的文本组件
    public Image IconImage; // 详情面板：显示物品图标的图片组件

    [Header("3D换装联动")]
    public long accountId = 19194472025L;  // 测试账号ID
    public RectTransform[] equipmentSlots; // 4个装备槽: [0]头盔 [1]身甲 [2]护手 [3]护腿
    [HideInInspector] public Item[] equippedItems;
    // Type→equipTypeIndex 映射: Type 1→2, 2→1, 3→3, 4→4
    public static readonly int[] TypeToEquipIndex = { 0, 2, 1, 3, 4 };
    // Type→3D Addressable Key 映射
    public static readonly int[] TypeTo3DKey = { 0, 1001, 1002, 1003, 1004 };

    // 起手式
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnTapBegin?.Invoke(eventData);
    }
    //开始拖拽
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != onlyResponder) return;

        var delta = eventData.position - eventData.pressPosition;
        if (delta.magnitude < dragDeadzone) return; // 死区内不取消计时器

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        isDrag = true;
    }
    //拖拽中
    public virtual void OnDrag(PointerEventData eventData)
    {
        //检测锁定
        if (eventData.pointerId != onlyResponder) return; 


        //判断拖拽是否处于长按状态
        if (isLongPress)
        {
            OnLongDragging?.Invoke(eventData);
        }
        else
        {
            OnShortDragging?.Invoke(eventData);
        }
    }
    //收起
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != onlyResponder) return;

        //停止计时器
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        // ── 抬起结算 ──
        if (isLongPress)
        {
            if (isDrag)
            {
                OnLongDragEnd?.Invoke(eventData);
            }
            else
            {
                OnLongClickEnd?.Invoke(eventData);
            }
        }
        else
        {
            if (isDrag)
            {
                OnShortDragEnd?.Invoke(eventData);
            }
            else
            {
                OnShortClickEnd?.Invoke(eventData);
            }
        }

        isDrag = false;
        onlyResponder = -1;
    }

    /// <summary>统一清理拖拽临时状态（长按结算分支共用）</summary>
    public void ClearDragState()
    {
        if (shadowItem != null)
        {
            shadowItem.SetActive(false);
            shadowItem.transform.SetParent(canvas != null ? canvas.transform : transform, false);
        }
        sourceObject = null;
        targetObject = null;
        sourceItem = null;
    }
}
}
