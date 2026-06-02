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
        public RectTransform prefab;
        public RectTransform detail;

        [Space]
        [Header("Grid")]
        public int rows = 5;
        public int totalCells = 20;
        public float cellWidth = 10f;
        public float maskHeight = 40f;
        public float containerFillHorizontal = 2f;
        public float containerFillUp = 12f;
        public float containerFillDown = 12f;

        [Space]
        [Header("视觉")]
        public Sprite containerSprite;
        public Sprite maskSprite;
        public Sprite cellSprite;
        public TMP_FontAsset itemFont;
    }
}
