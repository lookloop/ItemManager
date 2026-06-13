using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
public partial class Core
{
    // ── Inspector 配置 ──
    public float pressTime = 0.3f;
    public float scrollSpeed = 60f;
    public float edgeThreshold = 3f;
    public float turnThreshold = 0.5f;
    public float cellSize = 10f;
    public float fontSize = 3.9f;
    public float flipDuration = 0.5f;
    public Color shadowColor = new(0f, 0f, 0f, 0.9f);
    public TMP_FontAsset font;

    public ContainerSpec[] specs;

    // ── 运行时引用 ──
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public Container[] containers;
    [HideInInspector] public Container dragSourceContainer;
    [HideInInspector] public int dragSourceItemKey;

    // ── 拖拽幽灵 ──
    [HideInInspector] public RectTransform dragRect;
    [HideInInspector] public Image dragItem;
    [HideInInspector] public Image dragEdge;
    [HideInInspector] public TextMeshProUGUI dragCount;
    [HideInInspector] public RectTransform Shadow;
}
}
