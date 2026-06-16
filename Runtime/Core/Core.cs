using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{

public partial class Core : MonoBehaviour, IPointerDownHandler
{
    const string ContainerTag = "Container";

    /// <summary>
    /// Handles taps on empty space: closes all open detail panels and raises
    /// the clicked container to the top of the sibling order.
    ///
    /// Cell, container, and turn‑page interactions are handled by their own
    /// <c>TouchBase</c> components — <c>Core</c> does not participate in routing.
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId != 0) return;

        var clicked = eventData.pointerCurrentRaycast.gameObject;

        // Clicked Core itself (the full‑screen transparent receiver) or
        // clicked a container background (not a cell / button) → hide details
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

        // Raise the clicked container to the top
        if (clicked != null)
        {
            Transform t = clicked.transform;
            while (t != null)
            {
                if (t.CompareTag(ContainerTag))
                {
                    t.SetAsLastSibling();
                    break;
                }
                t = t.parent;
            }
        }
    }
}
}
