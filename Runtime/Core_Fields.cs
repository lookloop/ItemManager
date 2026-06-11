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

    // ── 跨组件共享的拖拽会话状态（SetPage 翻页时需知道谁在拖）──
    [HideInInspector] public Container sourceContainer;
    [HideInInspector] public int sourceItemKey;
}
}
