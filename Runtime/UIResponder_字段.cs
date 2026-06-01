using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class UIResponder
{
    public enum Mode { Scroll, Page, Fixed }

    // ════════════════════════════════════════
    // Inspector 字段 — 编辑器中配置
    // ════════════════════════════════════════

    [Header("Grid 参数")]
    public int rows = 5;                                 // Grid 行数
    public int cols = 4;                                 // Grid 列数
    public int totalItems = 20;                          // 物品总数（超过 cellCount 时分页）
    [Tooltip("格子边长 (mm)")]
    public float cellWidth = 10f;                        // 每个 Cell 的物理尺寸（mm），生成和缩放都靠它
    [Tooltip("物品图标边长 (mm)")]
    public float itemWidth = 8f;                         // Cell 内的物品图标尺寸（mm），略小于 cellWidth
    public float cellSpacing = 0f;                       // Cell 间距（预留）

    [Header("计时器")]
    public float timerValue = 0.3f;                      // 长按判定时间（秒），0.3s 后触发 B_LongPressStart

    [Header("视觉")]
    public Sprite backpackSprite;                        // 背包容器背景图
    public Sprite maskSprite;                            // Mask 区域背景图'
    public Sprite cellSprite;                            // Cell 格子默认背景图
    public TMPro.TMP_FontAsset itemFont;                 // 物品数量文字的字体

    [Header("Mask")]
    public float maskHeight = 40f;                       // Mask 可视区域高度（mm），超出部分被裁剪，决定了可滚动范围
    public float maskPosY = -8f;                         // Mask 的 Y 轴偏移（mm），微调 Mask 在容器中的位置

    [Header("面板")]
    public float horizontalPadding = 2f;                 // 容器左右内边距（mm）
    public float backpackExtraHeight = 12f;              // 容器额外高度（mm），容器总高 = maskHeight + 这个
    public bool autoBuild = true;                        // Start 时是否自动调用生成器 Build

    [Header("拖拽视觉")]
    public GameObject shadowItem;                        // 长按拖拽时悬停在目标格子上显示的阴影预制体

    [Header("详情面板")]
    public RectTransform Panel;                          // 点击 Item 时弹出的详情面板 RectTransform
    public TMPro.TextMeshProUGUI NameText;               // 详情面板 — 物品名称文字
    public TMPro.TextMeshProUGUI DescText;               // 详情面板 — 物品描述文字
    public Image IconImage;                              // 详情面板 — 物品图标 Image

    [Header("3D换装联动")]
    public long accountId = 19194472025L;                // 预留：关联的账号 ID

    // ════════════════════════════════════════
    // Container 管理
    // ════════════════════════════════════════

    [HideInInspector] public System.Collections.Generic.List<ContainerData> containers;
    // ↑ 所有容器注册列表。生成器创建的、预制体挂载的、自定义的都在这里。
    //   每个 ContainerData 含 container(RectTransform) + items(Item[])

    // ════════════════════════════════════════
    // 运行时字段 — Inspector 不可见，代码内部使用
    // ════════════════════════════════════════

    [HideInInspector] public Canvas canvas;
    // ↑ 所属 Canvas 引用（Awake 时自动获取），所有坐标转换的基准

    [HideInInspector] public bool isLongPress;
    // ↑ 长按标记。B_LongPressStart 设为 true。OnDrag 读它决定走长按拖拽还是短按拖拽。
    //   OnPointerUp 读它决定走长按结算还是短按结算

    [HideInInspector] public bool isDrag;
    // ↑ 拖拽标记。OnBeginDrag 设为 true（手指移动超过 Unity 内建阈值）。
    //   OnPointerUp 读它区分"点击"和"拖拽"。结算后重置为 false

    [HideInInspector] public Vector2 beginPosition;
    // ↑ 起手时手指在 Canvas 空间的位置（A_PointerDown 记录）。
    //   B_LongPressStart 用它将拾起的物品放到手指位置。
    //   C_ShortPressDrag.DragPanel/ScrollGrid 用它计算手指位移量

    [HideInInspector] public Vector2 endPosition;
    // ↑ 当前帧手指在 Canvas 空间的位置（C_ShortPressDrag 每帧更新）。
    //   totalDelta = endPosition - beginPosition

    [HideInInspector] public RectTransform dragTarget;
    // ↑ 拖拽目标：Container 拖拽时 = 容器 RectTransform；Grid 滚动时 = Grid RectTransform。
    //   A_PointerDown 根据 tag 赋值。C_ShortPressDrag 直接使用它 set anchoredPosition

    [HideInInspector] public Vector2 dragStartPos;
    // ↑ 拖拽目标在起手时的 anchoredPosition。A_PointerDown 记录。
    //   C_ShortPressDrag: 新位置 = dragStartPos + totalDelta

    [HideInInspector] public RectTransform gridTransform;
    // ↑ Grid RectTransform 引用（__背包生成器.Build 时赋值）。
    //   用于滚动计算、尺寸设置、坐标记录

    [HideInInspector] public RectTransform maskTransform;
    // ↑ Mask RectTransform 引用（__背包生成器.Build 时赋值）。
    //   用于获取可视区域高度（rect.height），计算滚动上限

    [HideInInspector] public RectTransform backpackPanel;
    // ↑ 背包容器面板 RectTransform 引用（__背包生成器.Build 时赋值）。
    //   用于 ApplyGridSize 设置容器总尺寸

    [HideInInspector] public int cellCount;
    // ↑ 总格子数 = rows × cols。Start 时计算，cellRegistry 数组长度由此决定

    [HideInInspector] public int cellsPerRow;
    // ↑ 每行格子数。Start 时计算。用于 ApplyCellPositions 计算行列位置。
    //   在 Scroll/Page 模式下 = cols；Fixed 模式下可能不同

    [HideInInspector] public GameObject[] cellRegistry;
    // ↑ 所有 Cell GameObject 的数组（__背包生成器.Build 时赋值）。
    //   索引即格子位置。D_ShortPressClick 用 IndexOf 定位点击的格子。
    //   D_LongPressEnd 用 IndexOf 定位交换的格子

    [HideInInspector] public Item[] items;
    // ↑ 物品数据数组，与 cellRegistry 一一对应。
    //   __背包初始化 写入/读取。D_ShortPressClick/D_LongPressEnd 通过它获取 Item 数据

    [HideInInspector] public int currentPage;
    // ↑ 当前页码（Page 模式下使用）

    [HideInInspector] public GameObject sourceItem;
    // ↑ 被拾起的物品 GameObject。B_LongPressStart 从 Cell 取第一个子物体赋值。
    //   C_LongPressDrag 让它跟随手指。D_LongPressEnd 交换/复位后由 ClearDragState 清空

    [HideInInspector] public GameObject sourceObject;
    // ↑ 起手时按到的 GameObject。A_PointerDown 记录。
    //   整个管线靠它获取 tag、查找 Container、计算索引。
    //   ClearDragState 结算后清空

    [HideInInspector] public GameObject targetObject;
    // ↑ 长按拖拽时当前悬停的 Cell。C_LongPressDrag 每帧通过射线检测更新。
    //   D_LongPressEnd 用它作为交换目标。ClearDragState 结算后清空

    [HideInInspector] public Coroutine timerCoroutine;
    // ↑ 长按计时器的 Coroutine 句柄。A_PointerDown 启动时赋值。
    //   OnBeginDrag/OnPointerUp 通过它 StopCoroutine 取消计时器。
    //   取消后置 null 标记遥控器已废
    // ↑ Item.Type 到 3D 装备资源的 key 映射表（预留）
}
}
