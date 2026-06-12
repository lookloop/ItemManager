using UnityEngine;
using UnityEngine.EventSystems;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 交互基类 — 所有可交互 UI 对象的统一入口。
    /// 子类按需 override：CellHandler / ContainerHandler / TurnPageHandler。
    /// 构建时由 ContainerBuilder 注入 core、container 引用，无需运行时查找。
    /// </summary>
    public abstract class TouchBase : MonoBehaviour,
        IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [HideInInspector] public Core core;
        [HideInInspector] public Container container;

        public virtual void OnPointerDown(PointerEventData eventData) { }
        public virtual void OnDrag(PointerEventData eventData)        { }
        public virtual void OnPointerUp(PointerEventData eventData)   { }
    }
}
