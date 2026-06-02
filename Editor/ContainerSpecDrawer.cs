using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lookloop.ItemManager.Editor
{
    /// <summary>
    /// ContainerSpec 的自定义 Inspector 显示。
    /// 只做一件事：prefab 始终可见，其余字段收到三角折叠下面。
    /// 分组间距由 ContainerSpec 自身的 [Header]/[Space] 控制，这里不干预。
    /// </summary>
    [CustomPropertyDrawer(typeof(ContainerSpec))] // 注册：告诉 Unity "碰到 ContainerSpec 就用我画"
    public class ContainerSpecDrawer : PropertyDrawer
    {
        // key = propertyPath（如 "mods.Array.data[0]"），保证数组里每个 mod 各自折叠
        static readonly Dictionary<string, bool> Foldouts = new();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // prefab + detail — 始终可见，不参与折叠
            EditorGUILayout.PropertyField(property.FindPropertyRelative("prefab"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("detail"));
            // 三角折叠按钮
            string key = property.propertyPath;
            if (!Foldouts.ContainsKey(key)) Foldouts[key] = false;
            Foldouts[key] = EditorGUILayout.Foldout(Foldouts[key], "参数");

            // 新建元素自动写默认值（Unity 数组新增不执行字段初始化器）
            if (property.FindPropertyRelative("totalCells").intValue == 0)
            {
                property.FindPropertyRelative("totalCells").intValue = 80;
                property.FindPropertyRelative("everyPageTotal").intValue = 40;
                property.FindPropertyRelative("rows").intValue = 5;
                property.FindPropertyRelative("cellWidth").floatValue = 10f;
                property.FindPropertyRelative("maskHeight").floatValue = 40f;
                property.FindPropertyRelative("containerFillHorizontal").floatValue = 2f;
                property.FindPropertyRelative("containerFillUp").floatValue = 8f;
                property.FindPropertyRelative("containerFillDown").floatValue = 4f;
            }

            if (Foldouts[key])
            {
                Prop(property, "totalCells",            "物品总数 (默认80)");
                Prop(property, "everyPageTotal",        "每页格子数 (默认40)");
                Prop(property, "rows",                  "每行格子数 (默认5)");
                Prop(property, "cellWidth",             "格子边长 (默认10)");
                Prop(property, "maskHeight",            "遮罩高度 (默认40)");
                Prop(property, "containerFillHorizontal", "水平内边距 (默认2)");
                Prop(property, "containerFillUp",       "上边距 (默认8)");
                Prop(property, "containerFillDown",     "下边距 (默认4)");
                Prop(property, "containerSprite");
                Prop(property, "maskSprite");
                Prop(property, "cellSprite");
            }
        }
        static void Prop(SerializedProperty parent, string name, string label = null)
        {
            var prop = parent.FindPropertyRelative(name);
            if (label != null)
                EditorGUILayout.PropertyField(prop, new GUIContent(label), true);
            else
                EditorGUILayout.PropertyField(prop, true);
        }
    }
}
