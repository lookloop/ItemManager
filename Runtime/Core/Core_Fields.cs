using UnityEngine;
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
    public float fontSize = 3.9f;
    public float flipDuration = 0.5f;
    public TMP_FontAsset font;

    public ContainerSpec[] specs;

    // ── 运行时引用 ──
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public Container[] containers;
    [HideInInspector] public DragTool dragTool;
    [HideInInspector] public DragSession dragSession;
}
}
