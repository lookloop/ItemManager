using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Lookloop.ItemManager
{
    /// <summary>
    /// 容器规格 — 构建容器的配方。
    /// 填入 UIResponder.mods[] 数组，每项生成一个独立容器。
    /// </summary>
    [Serializable]
    public class ContainerSpec
    {
        [Header("预制体 (可选)")]
        [Tooltip("不为空则直接 Instantiate，自动扫描 tag='Item' 的子对象作为 Cell 注册表")]
        public GameObject prefab;

        [Space]
        [Header("Grid")]
        public int rows = 5;
        public int cols = 4;
        public int totalItems = 20;
        [Tooltip("格子边长 (mm)")]
        public float cellWidth = 10f;
        [Tooltip("物品图标边长 (mm)")]
        public float itemWidth = 8f;
        public float cellSpacing = 0f;

        [Space]
        [Header("计时器")]
        public float timerValue = 0.3f;

        [Space]
        [Header("视觉")]
        public Sprite containerSprite;
        public Sprite maskSprite;
        public Sprite cellSprite;
        public TMP_FontAsset itemFont;

        [Space]
        [Header("Mask")]
        public float maskHeight = 40f;
        public float maskPosY = -8f;

        [Space]
        [Header("面板")]
        public float horizontalPadding = 2f;
        public float containerExtraHeight = 12f;
        [Tooltip("打开时自动归位到此坐标")]
        public Vector2 showPosition;

        [Space]
        [Header("拖拽视觉")]
        public GameObject shadowItem;

        [Space]
        [Header("详情面板")]
        [Tooltip("详情面板预制体（可选）。不为空则点物品时 Instantiate，点空时销毁")]
        public GameObject detailPanelPrefab;
        [Tooltip("↓↓↓ 预制体模式下可留空；非预制体模式下拖入场景引用")]
        public RectTransform detailPanel;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descText;
        public Image iconImage;
    }
}
