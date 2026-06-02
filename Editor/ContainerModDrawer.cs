using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lookloop.ItemManager.Editor
{
    /// <summary>
    /// ContainerMod 的自定义 Inspector 显示。
    /// 只做一件事：prefab 始终可见，其余字段收到三角折叠下面。
    /// 分组间距由 ContainerMod 自身的 [Header]/[Space] 控制，这里不干预。
    /// </summary>
    [CustomPropertyDrawer(typeof(ContainerMod))] // 注册：告诉 Unity "碰到 ContainerMod 就用我画"
    public class ContainerModDrawer : PropertyDrawer
    {
        // key = propertyPath（如 "mods.Array.data[0]"），保证数组里每个 mod 各自折叠
        static readonly Dictionary<string, bool> Foldouts = new();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // ① prefab — 永远在第一行，不参与折叠
            EditorGUILayout.PropertyField(property.FindPropertyRelative("prefab"));
            // ② 三角折叠按钮
            string key = property.propertyPath;                      // 每个属性的唯一路径标识
            if (!Foldouts.ContainsKey(key)) Foldouts[key] = false;    // 初次默认展开
            Foldouts[key] = EditorGUILayout.Foldout(Foldouts[key], "参数"); // 画三角

            if (Foldouts[key]) // 展开时才画以下字段
            {
                Prop(property, "rows");                // 行数
                Prop(property, "cols");                // 列数
                Prop(property, "totalItems");          // 物品总数
                Prop(property, "cellWidth");           // 格子边长
                Prop(property, "itemWidth");           // 物品图标边长
                Prop(property, "cellSpacing");         // 格子间距
                Prop(property, "timerValue");          // 长按计时器阈值
                Prop(property, "backpackSprite");      // 背包底图
                Prop(property, "maskSprite");          // Mask 区域图
                Prop(property, "cellSprite");          // 格子底图
                Prop(property, "itemFont");            // 物品数量字体
                Prop(property, "maskHeight");          // Mask 可视高度
                Prop(property, "maskPosY");            // Mask Y 偏移
                Prop(property, "horizontalPadding");   // 面板水平内边距
                Prop(property, "backpackExtraHeight"); // 面板额外高度
                Prop(property, "showPosition");        // 打开时归位的锚点坐标
                Prop(property, "shadowItem");          // 拖拽时跟随的阴影预制体
                Prop(property, "detailPanelPrefab");   // 详情面板预制体（动态 Instantiate）
                Prop(property, "detailPanel");         // 详情面板场景引用（非预制体模式）
                Prop(property, "nameText");            // 详情面板 — 物品名文本
                Prop(property, "descText");            // 详情面板 — 描述文本
                Prop(property, "iconImage");           // 详情面板 — 图标 Image
            }
        }
        static void Prop(SerializedProperty parent, string name)
        {
            EditorGUILayout.PropertyField(parent.FindPropertyRelative(name), true);
        }
    }
}
