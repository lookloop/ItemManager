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

        /// <summary>Bring the owning container to the top of the sibling order.</summary>
        protected void RaiseContainer()
        {
            if (container?.containerRect != null)
                container.containerRect.SetAsLastSibling();
        }

        public virtual void OnPointerDown(PointerEventData eventData) { }
        public virtual void OnDrag(PointerEventData eventData)        { }
        public virtual void OnPointerUp(PointerEventData eventData)   { }
    }
}
