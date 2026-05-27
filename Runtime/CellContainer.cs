using UnityEngine;

/// <summary>
/// Cell 容器组件 — 挂 Canvas 下自动生成面板+Grid+Cell。
/// mode: Scroll / Page / Fixed
/// </summary>
public class CellContainer : MonoBehaviour
{
    [Header("容器模式")]
    public enum Mode { Scroll, Page, Fixed }
    public Mode mode = Mode.Scroll;

    [Header("Grid 参数")]
    public int rows = 5;
    public int cols = 4;
    public int totalItems = 20;
    public float cellWidth = 10f;
    public float itemWidth = 8f;
    public float cellSpacing = 0f;

    [Header("视觉")]
    public Sprite backgroundSprite;
    public Sprite cellSprite;
    public TMPro.TMP_FontAsset itemFont;

    [Header("Mask (Scroll 模式)")]
    public float maskHeight = 200f;
    public float maskPosY = -20f;

    [Header("面板")]
    public float horizontalPadding = 4f;
    public float backpackExtraHeight = 40f;

    // 运行时
    [HideInInspector] public GameObject[] cells;
    [HideInInspector] public Item[] items;
    [HideInInspector] public int currentPage = 0;
    [HideInInspector] public RectTransform gridTransform;
    [HideInInspector] public RectTransform maskTransform;
    [HideInInspector] public RectTransform panelTransform;

    private UIResponder _responder;

    void Start()
    {
        _responder = GetComponent<UIResponder>();
        if (_responder == null)
            _responder = gameObject.AddComponent<UIResponder>();
        
        Build();
    }

    public void Build()
    {
        // 反向注入 UIResponder 参数
        _responder.cellCount = rows * cols;
        _responder.cellsPerRow = cols;
        _responder.cellWidth = cellWidth;
        _responder.itemWidth = itemWidth;
        _responder.cellSprite = cellSprite;
        _responder.backpackSprite = backgroundSprite;
        _responder.itemFont = itemFont;
        _responder.maskHeight = maskHeight;
        _responder.maskPosY = maskPosY;
        _responder.horizontalPadding = horizontalPadding;
        _responder.backpackExtraHeight = backpackExtraHeight;

        GridGenerator.Build(_responder);

        cells = _responder.cellRegistry;
        gridTransform = _responder.gridTransform;
        maskTransform = _responder.maskTransform;
        panelTransform = _responder.backpackPanel;

        // 初始化数据
        items = new Item[cells.Length];
        for (int i = 0; i < items.Length; i++)
            背包初始化.设置格子(_responder, i, null);
    }

    public void SetCell(int index, Item item)
    {
        if (index < 0 || index >= items.Length) return;
        背包初始化.设置格子(_responder, index, item);
    }

    public void NextPage()
    {
        currentPage++;
        SyncPage();
    }

    public void PrevPage()
    {
        if (currentPage > 0) currentPage--;
        SyncPage();
    }

    void SyncPage()
    {
        // 翻页: 偏移 Key 重新同步
        for (int i = 0; i < cells.Length; i++)
            背包初始化.设置格子(_responder, i, null);
        // 外部通过 SetCell 填充
    }
}