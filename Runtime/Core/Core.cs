using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public partial class Core : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// Handles taps on empty space: closes all open detail panels.
    /// Container raising is handled by each <c>TouchBase</c> subclass in its
    /// own <c>OnPointerDown</c>, so <c>Core</c> only deals with blank‑space cleanup.
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        var clicked = eventData.pointerCurrentRaycast.gameObject;

        if (containers != null)
        {
            foreach (var c in containers)
            {
                var dr = c?.detailRect;
                if (dr != null && dr.gameObject.activeSelf &&
                    (clicked == null || !clicked.transform.IsChildOf(dr)))
                {
                    dr.gameObject.SetActive(false);
                }
            }
        }
    }
}
}
