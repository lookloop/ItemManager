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

            if (Foldouts[key])
            {
                Prop(property, "totalCells");
                Prop(property, "everyPageTotal");
                Prop(property, "rows");
                Prop(property, "cellWidth");
                Prop(property, "maskHeight");
                Prop(property, "containerFillHorizontal");
                Prop(property, "containerFillUp");
                Prop(property, "containerFillDown");
                Prop(property, "containerSprite");
                Prop(property, "maskSprite");
                Prop(property, "cellSprite");
            }
        }
        static void Prop(SerializedProperty parent, string name)
        {
            EditorGUILayout.PropertyField(parent.FindPropertyRelative(name), true);
        }
    }
}
