using UnityEngine;
using TMPro;

namespace Lookloop.ItemManager
{
public partial class Core
{
    public float pressTime = 0.3f;
    public TMP_FontAsset font;

    public ContainerSpec[] specs;

    [HideInInspector] public Canvas canvas;

    [HideInInspector] public Container[] containers;

}
}
