using System.Threading.Tasks;
using UnityEngine;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Abstract base for detail panels. Attach a concrete implementation to your
    /// <c>detailRect</c> prefab; when the player taps a cell, <c>Fill</c> is called
    /// so you can render custom item details (stats, description, etc.).
    /// </summary>
    public abstract class DetailBase : MonoBehaviour
    {
        [System.NonSerialized] public Core core;
        [System.NonSerialized] public Container container;

        public abstract Task Fill(Container container, int itemKey);
    }
}
