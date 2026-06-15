using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
public partial class Core
{
    // ── Inspector 配置 ──

    public float cellSize = 10f;
    public float pressTime = 0.3f;
    public float fontSize = 3.9f;
    public TMP_FontAsset font;
    public float retainTime = 1800f;
    public float checkTime = 300f;
    public Color shadowColor = new(0f, 0f, 0f, 0.9f);
    public float scrollSpeed = 60f;
    public float flipDistance = 3f;
    public float flipCool = 0.5f;
    public float flipDuration = 0.5f;
    

    public ContainerSpec[] specs;

    // ── 运行时引用 ──
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public Container[] containers;
    [HideInInspector] public Container sourceContainer;
    [HideInInspector] public int sourceItemKey;

    // ── 拖拽幽灵 ──
    [HideInInspector] public RectTransform dragParent;
    [HideInInspector] public Image dragItem;
    [HideInInspector] public Image dragEdge;
    [HideInInspector] public TextMeshProUGUI dragCount;
    [HideInInspector] public RectTransform Shadow;
    [HideInInspector] public TextMeshProUGUI tmpTip;
}
}
