using UnityEngine;
using UnityEngine.UI;

namespace Lookloop.ItemManager
{
public partial class Core
{
    /// <summary>
    /// One‑time setup: stretch Core to fill the canvas, push it to the back,
    /// and add a transparent <c>Image</c> so it can receive blank‑space clicks.
    /// </summary>
    void SundriesInit()
    {
        // Cache the canvas reference (needed later for world‑camera calculations)
        canvas = GetComponentInParent<Canvas>();

        // Stretch to fill the entire canvas
        var rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        // Push to the bottom of the sibling order so it doesn't occlude other UI
        rt.SetAsFirstSibling();

        // Transparent image that catches raycasts on empty space
        var img = gameObject.AddComponent<Image>();
        img.color = Color.clear;
        img.raycastTarget = true;
    }
}
}
