using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 容器规格 — 构建容器的配方。
    /// 填入 Core.mods[] 数组，每项生成一个独立容器。
    /// </summary>
    [Serializable]
    public class ContainerSpec
    {
        [Header("预制体 (可选)")]
        [Tooltip("不为空则直接 Instantiate，自动扫描 tag='Item' 的子对象作为 Cell 注册表")]
        public RectTransform prefabRect;
        public RectTransform detailRect;

        [Space]
        [Header("Grid")]
        public int totalItems = 80;
        public int everyPageCells = 40;
        public int row = 5;
        public float cellWidth = 10f;
        public float maskHeight = 40f;
        public float containerFillHorizontal = 2f;
        public float containerFillUp = 8f;
        public float containerFillDown = 4f;

        [Space]
        [Header("过滤")]
        [Tooltip("物品准入过滤器，为空则无限制。")]
        public SetItemBase itemFilter;

        [Space]
        [Header("翻页")]
        public float pageTextWidth = 24f;
        public float pageTextHeight = 4f;

        [Space]
        [Header("视觉")]
        public Sprite containerSprite;
        public Sprite maskSprite;
        public Sprite cellSprite;
    }
}
