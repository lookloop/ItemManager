using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// Container blueprint — a recipe that drives procedural UI construction.
    /// Add entries to Core.specs[]; each entry produces one independent container.
    /// </summary>
    [Serializable]
    public class ContainerSpec
    {
        [Header("Prefab (optional)")]
        [Tooltip("If set, instantiate this prefab instead of building from scratch. "
            + "Child transforms tagged 'Cell' are auto-detected as the cell registry.")]
        public RectTransform prefabRect;
        public RectTransform detailRect;

        [Space]
        [Header("Grid")]
        public int totalItems = 80;
        public int everyPageCells = 40;
        public int row = 5;
        public float maskHeight = 40f;
        public float containerFillHorizontal = 2f;
        public float containerFillUp = 8f;
        public float containerFillDown = 4f;

        [Space]
        [Header("Filter")]
        [Tooltip("Item-admission filter. Leave empty for unrestricted. "
            + "Choose a derived type from the drop-down; its fields expand inline.")]
        [SerializeReference]
        public SetItemBase itemFilter;

        [Space]
        [Header("Pagination")]
        public float pageTextWidth = 24f;
        public float pageTextHeight = 4f;

        [Space]
        [Header("Visuals")]
        public Sprite containerSprite;
        public Sprite maskSprite;
        public Sprite cellSprite;
    }
}
