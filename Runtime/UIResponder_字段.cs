using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class UIResponder
{
    public enum Mode { Scroll, Page, Fixed }

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

    [HideInInspector] public System.Collections.Generic.List<ContainerData> containers;

    [HideInInspector] public Canvas canvas;
    [HideInInspector] public Camera uiCamera;

    [HideInInspector] public RectTransform gridTransform;

    [HideInInspector] public RectTransform maskTransform;

    [HideInInspector] public RectTransform backpackPanel;

    [HideInInspector] public int cellCount;

    [HideInInspector] public int cellsPerRow;

    [HideInInspector] public GameObject[] cellRegistry;

    [HideInInspector] public Item[] items;

    [HideInInspector] public int currentPage;

    [HideInInspector] public string currentTag;

}
}
