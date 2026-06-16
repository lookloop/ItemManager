using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public partial class Core : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// Blank-space tap receiver: hides <b>all</b> open detail panels.
    /// Container-relative taps are intercepted by their own <c>TouchBase</c>,
    /// which calls <c>FocusContainer()</c> and suppresses this fallback.
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        var clicked = eventData.pointerCurrentRaycast.gameObject;

        // Only Core's transparent image receives blank-space hits;
        // container / cell / button hits are handled elsewhere.
        if (clicked != gameObject) return;

        if (containers != null)
        {
            foreach (var c in containers)
            {
                var dr = c?.detailRect;
                if (dr != null && dr.gameObject.activeSelf)
                    dr.gameObject.SetActive(false);
            }
        }
    }
}
}
