using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Base class for all interactable UI elements. Implements the three pointer
    /// events so subclasses only override what they need.
    ///
    /// Concrete types: CellTouch / ContainerTouch / TurnPageTouch.
    /// References to <c>core</c> and <c>container</c> are injected at build time
    /// by the container builder — no runtime lookups required.
    /// </summary>
    public abstract class TouchBase : MonoBehaviour,
        IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [HideInInspector] public Core core;
        [HideInInspector] public Container container;

        /// <summary>
        /// Bring the owning container to the top and hide detail panels of all
        /// <b>other</b> containers. Call this at the very beginning of every
        /// <c>OnPointerDown</c> override.
        /// </summary>
        protected void FocusContainer()
        {
            if (container?.containerRect != null)
                container.containerRect.SetAsLastSibling();

            if (core?.containers != null)
            {
                foreach (var c in core.containers)
                {
                    if (c != container)
                    {
                        var dr = c?.detailRect;
                        if (dr != null && dr.gameObject.activeSelf)
                            dr.gameObject.SetActive(false);
                    }
                }
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData) { }
        public virtual void OnDrag(PointerEventData eventData)        { }
        public virtual void OnPointerUp(PointerEventData eventData)   { }
    }
}
