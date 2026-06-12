using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lookloop.ItemManager.Editor
{
    [CustomPropertyDrawer(typeof(ContainerSpec))]
    public class ContainerSpecDrawer : PropertyDrawer
    {
        static readonly Dictionary<string, bool> Foldouts = new();

        // 缓存所有 SetItemBase 派生类型
        static Type[] filterTypes;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // prefab + detail — 始终可见
            EditorGUILayout.PropertyField(property.FindPropertyRelative("prefabRect"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("detailRect"));

            // itemFilter — 手动绘制 [SerializeReference] 类型选择器
            DrawFilterField(property.FindPropertyRelative("itemFilter"));

            // 三角折叠按钮
            string key = property.propertyPath;
            if (!Foldouts.ContainsKey(key)) Foldouts[key] = false;
            Foldouts[key] = EditorGUILayout.Foldout(Foldouts[key], "参数");

            // 新建元素自动写默认值
            if (property.FindPropertyRelative("totalItems").intValue == 0)
            {
                property.FindPropertyRelative("totalItems").intValue = 80;
                property.FindPropertyRelative("everyPageCells").intValue = 40;
                property.FindPropertyRelative("row").intValue = 5;
                property.FindPropertyRelative("cellWidth").floatValue = 10f;
                property.FindPropertyRelative("maskHeight").floatValue = 40f;
                property.FindPropertyRelative("containerFillHorizontal").floatValue = 2f;
                property.FindPropertyRelative("containerFillUp").floatValue = 8f;
                property.FindPropertyRelative("containerFillDown").floatValue = 4f;
                property.FindPropertyRelative("pageTextWidth").floatValue = 24f;
                property.FindPropertyRelative("pageTextHeight").floatValue = 4f;
            }

            if (Foldouts[key])
            {
                Prop(property, "totalItems",            "物品总数 (默认80)");
                Prop(property, "everyPageCells",        "每页格子数 (默认40)");
                Prop(property, "row",                   "每行格子数 (默认5)");
                Prop(property, "cellWidth",             "格子边长 (默认10)");
                Prop(property, "maskHeight",            "遮罩高度 (默认40)");
                Prop(property, "containerFillHorizontal","水平内边距 (默认2)");
                Prop(property, "containerFillUp",       "上边距 (默认8)");
                Prop(property, "containerFillDown",     "下边距 (默认4)");
                Prop(property, "pageTextWidth",         "翻页输入宽 (默认24)");
                Prop(property, "pageTextHeight",        "翻页输入高 (默认4)");
                Prop(property, "containerSprite");
                Prop(property, "maskSprite");
                Prop(property, "cellSprite");
            }
        }

        // ── [SerializeReference] 手动类型选择器 ──
        void DrawFilterField(SerializedProperty prop)
        {
            CacheFilterTypes();

            object current = prop.managedReferenceValue;
            int currentIndex = -1;
            string[] names;

            if (filterTypes.Length == 0)
            {
                // 没有派生类 → 只显示提示
                names = new[] { "(无可用类型)" };
            }
            else
            {
                names = new string[filterTypes.Length + 1];
                names[0] = "(None)";
                for (int i = 0; i < filterTypes.Length; i++)
                {
                    names[i + 1] = filterTypes[i].Name;
                    if (current != null && current.GetType() == filterTypes[i])
                        currentIndex = i + 1;
                }
            }

            EditorGUI.BeginChangeCheck();
            int selected = EditorGUILayout.Popup(
                "Item Filter",
                currentIndex == -1 ? 0 : currentIndex,
                names);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(prop.serializedObject.targetObject, "Change Item Filter");
                if (selected <= 0 || filterTypes.Length == 0)
                    prop.managedReferenceValue = null;
                else
                    prop.managedReferenceValue = Activator.CreateInstance(filterTypes[selected - 1]);
            }

            // 有值时展开子字段
            if (current != null)
            {
                EditorGUI.indentLevel++;
                SerializedProperty child = prop.Copy();
                SerializedProperty end = child.GetEndProperty();
                child.NextVisible(true);
                while (!SerializedProperty.EqualContents(child, end))
                {
                    EditorGUILayout.PropertyField(child, true);
                    child.NextVisible(false);
                }
                EditorGUI.indentLevel--;
            }
        }

        static void CacheFilterTypes()
        {
            if (filterTypes != null) return;

            var list = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (t.IsSubclassOf(typeof(SetItemBase)) && !t.IsAbstract)
                        list.Add(t);
                }
            }
            filterTypes = list.ToArray();
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
